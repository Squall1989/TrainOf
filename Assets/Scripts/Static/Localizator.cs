using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Localizator 
{
    public delegate void LangDelegate();
    private static Dictionary<string, string[]> localizationTable;
    private static int currLang = 0;
    public static LangDelegate langDelegate;

    public static void Init()
    {
        localizationTable = new Dictionary<string, string[]>();
        LoadLocalizationFile();
    }

    public static void Start()
    {
        currLang = PlayerPrefs.GetInt("Language");
        ChangeLang(currLang);
    }

    public static string GetLangChangeItem(string itemCode)
    {
        return localizationTable[itemCode][currLang];
    }

    public static void ChangeLang(int num_)
    {
        currLang = num_;
        langDelegate.Invoke();
    }

    private static void LoadLocalizationFile()
    {
        string localStr = Resources.Load<TextAsset>("localization").text;
        string[] lineSeparator = new string[] { "\r\n" };
        string[] separateLines = localStr.Split(lineSeparator, System.StringSplitOptions.None);

        string[] rowSeparator = new string[] { "\",\"", "\",", ",\"" };
        for (int s = 0; s < separateLines.Length; s++)
        {
            string[] rows = separateLines[s].TrimEnd('"').Split(rowSeparator, System.StringSplitOptions.None);

            if (rows.Length == 1)
                rows = separateLines[s].Split(',');

            if (s == 0)//lang ru en
                continue;

            string key = rows[0];
            string[] values = new string[rows.Length - 1];
            for (int i = 1; i < rows.Length; i++)
                values[i - 1] = rows[i];

            localizationTable.Add(key, values);
        }
    }
}
