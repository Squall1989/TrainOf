using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager 
{
    public const int levelsForCity = 20, chapterForCity = 3, peopleMax = 3;
    private static ChapterData[][] levelDatas;// = new ChapterData[5][];
    //private static PlayLevelPos playingLevel;
    public static void SaveProgress(PlayLevelPos levelPos, int peopleCollect_)
    {
        
        int[] peopleCollectArr = levelDatas[levelPos.cityNum][levelPos.ChapterNum].peoplesCollected;
        peopleCollectArr[levelPos.levelNum] = peopleCollect_;

        SaveStrProgress();
        
    }

    private static void SaveStrProgress()
    {
        string[][] saveStrs = new string[levelDatas.GetLength(0)][];
        for (int j = 0; j < saveStrs.GetLength(0); j++)
        {
            int levDataLen = levelDatas[j].Length;
            saveStrs[j] = new string[levDataLen];
            for (int i = 0; i < saveStrs[j].Length; i++)
            {
                saveStrs[j][i] = JsonUtility.ToJson(levelDatas[j][i]);
            }
        }

        FileManager.SaveToFile(saveStrs);
    }

    public static int GetChapterProgress(PlayLevelPos currLevel)
    {
        int[] PParr = levelDatas[currLevel.cityNum][currLevel.ChapterNum].peoplesCollected;
        float total = PParr.Length * peopleMax;
        float collect = 0;
        for(int pp = 0; pp < PParr.Length; pp++)
        {
            collect += PParr[pp];
        }
        float percent = (collect / total) * 100f;
        return (int)percent;
    }

    private static bool ChapterComplete(PlayLevelPos playingLevel)
    {
        int[] peopleCollectArr = levelDatas[playingLevel.cityNum][playingLevel.ChapterNum].peoplesCollected;

        bool isComplete = true;
        for (int pc = 0; pc < peopleCollectArr.Length; pc++)
        {
            if(peopleCollectArr[pc] < 2)//Глава не завершена если меньше 2
            {
                isComplete = false;
                break;
            }
        }
        Debug.Log("Chapter complete: " + isComplete);
        return isComplete;
    }

    private static PlayLevelPos GetLevelNotComplete(PlayLevelPos playingLevel)
    {
        int[] peopleCollectArr = levelDatas[playingLevel.cityNum][playingLevel.ChapterNum].peoplesCollected;

        for (int pc = 0; pc < peopleCollectArr.Length; pc++)
        {
            if (peopleCollectArr[pc] < 2)//Глава не завершена если меньше 2
            {
                playingLevel.levelNum = pc;

                break;
            }
        }
        return playingLevel;
    }

    public static PlayLevelPos NextLevel(PlayLevelPos playingLevel)
    {
        int[] peopleCollectArr = levelDatas[playingLevel.cityNum][playingLevel.ChapterNum].peoplesCollected;
        
        if (ChapterComplete(playingLevel))//Глава пройдена
        {
            playingLevel.levelNum = 0;
            if (playingLevel.ChapterNum < levelDatas[playingLevel.cityNum].Length - 1)//Город пройден
            {
                levelDatas[playingLevel.cityNum][playingLevel.ChapterNum + 1].OpenChapter();
                playingLevel.ChapterNum += 1;
                SaveStrProgress();
            }
            else
            {
                //City open
            }
        }
        else if(playingLevel.levelNum < peopleCollectArr.Length - 1)
        {
            playingLevel.levelNum += 1;
        }
        else//Если последний уровень и глава не завершена - ищем незавершенный уровень
        {
            playingLevel = GetLevelNotComplete(playingLevel);
        }
        return playingLevel;
    }

    private static void CreateData()
    {
        levelDatas = new ChapterData[5][];  
        for (int lev = 0; lev < levelDatas.Length; lev++)
        {
            if (lev == 2)//На втором городе всего 2 главы
                levelDatas[lev] = new ChapterData[2];
            else
                levelDatas[lev] = new ChapterData[chapterForCity];

            for (int chap = 0; chap < levelDatas[lev].Length; chap++)
            {
                //int[] peopLevArr = new int[levelsForCity];
                //peopLevArr.Initialize();

                levelDatas[lev][chap] = new ChapterData(lev, chap);
                
            }
        }
    }

    public static PlayLevelPos GetStartLevel()
    {

        int playChapter = 0, playLevel = 0, playCity = 0;
        for(int city = 0; city < 1; city++)//ToDo
        for(int chap = 0; chap < levelDatas[city].Length; chap++)
        {
            int[] PPC = levelDatas[city][chap].peoplesCollected;
            bool isPlayChapter = false;
            for (int level = 0; level < PPC.Length; level++)
            {
                if (PPC[level] < 2)//Есть незаконченный уровень
                {
                    playLevel = level;
                    isPlayChapter = true;
                       
                    break;
                }
            }
            if (isPlayChapter)
            {
                playChapter = chap;
                break;
            }
        }
        //Debug.Log("playLevel: " + playLevel);
        //Debug.Log("playChapter: " + playChapter);
        return new PlayLevelPos(playLevel, playChapter, playCity);
    }

    public static ChapterData[] GetCityInfo(int num_)
    {
        return levelDatas[num_];
    }

    public static ChapterData GetChapterInfo(int chap_, int city_)
    {
        return levelDatas[city_][chap_];
    }

    public static void LoadProgress()
    {
        string[] strFromFile = FileManager.LoadFromFile();
        CreateData();

        if (strFromFile == null || strFromFile.Length == 0)
        {
            SaveProgress(new PlayLevelPos(),0);
            return;
        }
        int line = 0;
        for(int l = 0; l < levelDatas.GetLength(0); l++)
        for(int c = 0; c < levelDatas[l].Length; c++)
        {
            levelDatas[l][c] = JsonUtility.FromJson<ChapterData>(strFromFile[line++]);
        }


    }
}

[System.Serializable]
public struct ChapterData
{
    public bool isOpen;
    public int cityNum, chapterNum;
    public int[] peoplesCollected;
    public ChapterData(int city_, int chapter_)
    {
        isOpen = false;
        cityNum = city_;
        int levelsCount = 20;
        if (city_ == 0 && chapter_ == 0)
        {
            levelsCount = 4;
            isOpen = true;
        }
        chapterNum = chapter_;
        peoplesCollected = new int[levelsCount];
    }
    /*public ChapterData(int city_, int chapter_, int[] peoplesArray)
    {
        isOpen = (city_ == 0 && chapter_ == 0);
        cityNum = city_;
        chapterNum = chapter_;
        peoplesCollected = peoplesArray;
    }
    */
    public void OpenChapter()
    {
        Debug.Log("Chapter open: " + chapterNum);
        isOpen = true;
    }
}
public struct PlayLevelPos
{
    public int levelNum, ChapterNum, cityNum;
    public PlayLevelPos(int LevelNum_, int chapterNum_, int cityNum_)
    {
        levelNum = LevelNum_;
        ChapterNum = chapterNum_;
        cityNum = cityNum_;
    }
}
