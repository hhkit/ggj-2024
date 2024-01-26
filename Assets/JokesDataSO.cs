using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Joke
{
    public Joke(string[] lines, string[] tags)
    {
        Lines = lines;
        Tags = tags;
    }
    // public JokeLevel jokeLevel;
    public string[] Lines { get; private set; }
    public string[] Tags { get; private set; }
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

    [EasyButtons.Button]
    void RefreshSheet()
    {
        string url = $"https://docs.google.com/spreadsheets/d/{m_GoogleSheetsID}/gviz/tq?tqx=out:csv&sheet={m_GoogleSheetsSheetName}";
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

        Regex lineRegex = new Regex(@"(?<=^|,)""(.*?)""(?=$|,)");
        Regex tagDelimiterRegex = new Regex(@",\s*");

        string[] lines = csv.Split("\n");
        for (int r = m_GoogleSheetsOffsetRows; r < lines.Length; r++)
        {
            string line = lines[r];
            var matches = lineRegex.Matches(line);

            Joke joke = new Joke(
                matches[m_GoogleSheetsOffsetCols].Value.Split(Environment.NewLine),
                tagDelimiterRegex.Split(matches[m_GoogleSheetsOffsetCols + 1].Value));
            m_Jokes.Add(joke);
        }
    }
#endif // UNITY_EDITOR
}
