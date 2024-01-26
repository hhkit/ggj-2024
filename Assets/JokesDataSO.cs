using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Joke
{
    public Joke(string[] lines, string[] tags, bool isLame)
    {
        m_Lines = lines;
        m_Tags = tags;
        m_IsLame = isLame;
    }
    // public JokeLevel jokeLevel;
    [SerializeField]
    string[] m_Lines;
    public string[] Lines => m_Lines;

    [SerializeField]
    string[] m_Tags;
    public string[] Tags => m_Tags;
    [SerializeField]
    bool m_IsLame;
    public bool IsLame => m_IsLame;
    public bool IsFunny => !m_IsLame;
}

[CreateAssetMenu(fileName = "JokesData", menuName = "JokesData")]
public class JokesDataSO : ScriptableObject
{
    [SerializeField]
    List<Joke> m_Jokes;

    public List<Joke> Jokes => m_Jokes;

#if UNITY_EDITOR
    public string m_GoogleSheetsID;
    public string m_GoogleSheetsSheetName = "Sheet1";
    public int m_GoogleSheetsOffsetRows = 0;
    public int m_GoogleSheetsOffsetCols = 0;
    
    /**
     * UPDATE THIS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
     */
    const int GoogleSheetsNumCols = 3;

    [EasyButtons.Button]
    void RefreshSheet()
    {
        string url = $"https://docs.google.com/spreadsheets/d/{m_GoogleSheetsID}/gviz/tq?tqx=out:csv&sheet={m_GoogleSheetsSheetName}&range={(char)('A' + m_GoogleSheetsOffsetCols)}:{(char)('A' + m_GoogleSheetsOffsetCols + GoogleSheetsNumCols - 1)}";
        var req = UnityWebRequest.Get(url);
        var op = req.SendWebRequest();
        op.completed += OnGetSheet;
    }

    void OnGetSheet(AsyncOperation op)
    {
        var req = (op as UnityWebRequestAsyncOperation).webRequest;
        if (req.result == UnityWebRequest.Result.Success)
        {
            string csv = req.downloadHandler.text;
            ParseCSV(csv);
        }
    }

    void ParseCSV(string csv)
    {
        m_Jokes.Clear();

        Regex cellRegex = new Regex(@"(?<=^|,)""(.*?)""(?=$|,)", RegexOptions.Multiline | RegexOptions.Singleline);
        Regex tagDelimiterRegex = new Regex(@",\s*");

        var matches = cellRegex.Matches(csv);
        int r = m_GoogleSheetsOffsetRows;
        while (true)
        {
            int i = r * GoogleSheetsNumCols;

            if (i >= matches.Count)
                break;

            Joke joke = new Joke(
                matches[i].Groups[1].Value.Split('\n'),
                tagDelimiterRegex.Split(matches[i + 1].Groups[1].Value),
                matches[i + 2].Groups[1].Value == "TRUE");
            m_Jokes.Add(joke);

            ++r;
        }
    }
#endif // UNITY_EDITOR
}
