using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.Networking;
#endif // UNITY_EDITOR

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

[Serializable]
public class QuoteData
{
    [SerializeField]
    string[] m_Lines;
    public string[] Lines => m_Lines;

    [SerializeField]
    string m_Context;
    public string Context => m_Context;
    public QuoteData(string[] lines, string context)
    {
        m_Lines = lines;
        m_Context = context;
    }
}

[Serializable()]
public struct SheetRequest
{
    public string GoogleSheetsID;
    public string GoogleSheetsSheetName;
    public int GoogleSheetsOffsetRows;
    public int GoogleSheetsOffsetCols;
    public int GoogleSheetsNumCols;

    public SheetRequest(string id)
    {
        GoogleSheetsID = id;
        GoogleSheetsSheetName = "Sheet1";
        GoogleSheetsOffsetRows = 0;
        GoogleSheetsOffsetCols = 0;
        GoogleSheetsNumCols = 3;
    }
}
public static class ListExt
{
    public static T GetRandomWhere<T>(this List<T> list, Func<T, bool> pred)
    {
        return list.Where(pred).OrderBy(s => Guid.NewGuid()).First();
    }
} 

[CreateAssetMenu(fileName = "JokesData", menuName = "JokesData")]
public class JokesDataSO : ScriptableObject
{
    [SerializeField]
    List<Joke> m_Jokes;
    [SerializeField]
    List<QuoteData> m_KingLines;
    [SerializeField]
    List<QuoteData> m_PlayerLines;
    [SerializeField]
    List<QuoteData> m_JesterLines;

    public List<Joke> Jokes => m_Jokes;
    public List<QuoteData> KingLines => m_KingLines;
    public List<QuoteData> PlayerLines => m_PlayerLines;
    public List<QuoteData> JesterLines => m_JesterLines;



#if UNITY_EDITOR
    public SheetRequest m_JokeRequest;
    public SheetRequest m_KingLineRequest;
    public SheetRequest m_PlayerLineRequest;
    public SheetRequest m_JesterLineRequest;

    [EasyButtons.Button]
    void RefreshSheet()
    {
        GetSheet(m_JokeRequest, (string csv) => {
            m_Jokes = ParseCsvAsJokes(csv, m_JokeRequest);
            SubmitAssetChanges();
        });
        GetSheet(m_KingLineRequest, (string csv) =>
        {
            m_KingLines = ParseCsvAsQuotes(csv, m_KingLineRequest);
            SubmitAssetChanges();
        });
        GetSheet(m_PlayerLineRequest, (string csv) =>
        {
            m_PlayerLines = ParseCsvAsQuotes(csv, m_PlayerLineRequest);
            SubmitAssetChanges();
        });
        GetSheet(m_JesterLineRequest, (string csv) =>
        {
            m_JesterLines = ParseCsvAsQuotes(csv, m_JesterLineRequest);
            SubmitAssetChanges();
        });
    }

    void GetSheet(SheetRequest request, Action<string> OnCsvLoaded)
    {
        string url = $"https://docs.google.com/spreadsheets/d/{request.GoogleSheetsID}/gviz/tq?tqx=out:csv&sheet={request.GoogleSheetsSheetName}&range={(char)('A' + request.GoogleSheetsOffsetCols)}:{(char)('A' + request.GoogleSheetsOffsetCols + request.GoogleSheetsNumCols - 1)}";
        var req = UnityWebRequest.Get(url);
        var op = req.SendWebRequest();
        op.completed += (AsyncOperation op) =>
        {
            var req = (op as UnityWebRequestAsyncOperation).webRequest;
            if (req.result == UnityWebRequest.Result.Success)
            {
                string csv = req.downloadHandler.text;
                OnCsvLoaded(csv);
            }
        };
    }

    List<Joke> ParseCsvAsJokes(string csv, SheetRequest requestData)
    {
        var jokes = new List<Joke>();

        Regex cellRegex = new Regex(@"(?<=^|,)""(.*?)""(?=$|,)", RegexOptions.Multiline | RegexOptions.Singleline);
        Regex tagDelimiterRegex = new Regex(@",\s*");

        var matches = cellRegex.Matches(csv);
        int r = requestData.GoogleSheetsOffsetRows;
        while (true)
        {
            int i = r * requestData.GoogleSheetsNumCols;
            ++r;

            if (i >= matches.Count)
                break;

            var lines = matches[i].Groups[1].Value.Replace("\"\"", "\"").Split('\n').ToList();
            for (int l = 0; l < lines.Count; ++l)
            {
                lines[l] = lines[l].Trim();
                if (lines[l].Length == 0)
                    lines.RemoveAt(l--);
            }
            if (lines.Count == 0)
                continue;

            var tags = tagDelimiterRegex.Split(matches[i + 1].Groups[1].Value);
            var isLame = matches[i + 2].Groups[1].Value == "TRUE";

            jokes.Add(new(lines.ToArray(), tags, isLame));
        }

        return jokes;
    }

    List<QuoteData> ParseCsvAsQuotes(string csv, SheetRequest requestData)
    {
        var list = new List<QuoteData>();

        Regex cellRegex = new Regex(@"(?<=^|,)""(.*?)""(?=$|,)", RegexOptions.Multiline | RegexOptions.Singleline);

        var matches = cellRegex.Matches(csv);
        int r = requestData.GoogleSheetsOffsetRows;
        while (true)
        {
            int i = r * requestData.GoogleSheetsNumCols;

            if (i >= matches.Count)
                break;

            var lines = matches[i].Groups[1].Value.Replace("\"\"", "\"").Split('\n');
            var tag = matches[i + 1].Groups[1].Value.Trim();

            list.Add(new(lines, tag));

            ++r;
        }

        return list;
    }

    void SubmitAssetChanges()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif // UNITY_EDITOR
}