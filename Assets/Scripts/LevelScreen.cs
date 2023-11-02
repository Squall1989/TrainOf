using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScreen : MonoBehaviour
{
    public static LevelScreen levelScreen;
    //private CityStatus currCity;
    public CityStatus tempCity;
    public Text chapPassInf;
    public Transform playImage;
    public RectTransform contentTR;
    public Sprite completeBut, completePres, notComplete, notCompletePres, playingBut, playingPres, closeBut;
    public Input_Menu menuStart;

    private const float distChaptersNorm = 1933f, distChaptersMin = -610f + 1303f, contentMin = 2430f, contentMax = 3000f;
    //public CityStatus[] cities;
    public chapter[] chapters;

    private void Awake()
    {
        levelScreen = this;
    }
    private void OnEnable()
    {
        Localizator.langDelegate += ChangeLanguage;
    }
    private void OnDisable()
    {
        Localizator.langDelegate -= ChangeLanguage;
    }

    void Start()
    {
        
        //Image peopleImage = buttonsParent.GetChild(0).GetChild(1).GetComponent<Image>();
        //peopleImageWidth = peopleImage.rectTransform.sizeDelta.x;
    }

    public void ReInitButtons()
    {
        int playCity = GameplayController.controller.getLevel().cityNum;
        CityStatus.statusMap.clickCity(playCity);
    }
    /*
    public CityStatus GetCity()
    {
        return currCity;
    }
    */
    private void ChangeLanguage()
    {
        //Debug.Log("Lang change");
        //main_choose_level.text = Localizator.GetLangChangeItem("main_choose_level");
    }


    public void InitChapters(int cityNum)
    {
        //currCity = cityInf;
        PlayLevelPos levelPos = SaveManager.GetStartLevel();
        int totalChapterPeoples = 0;
        int collectChapterPeoples = 0;
        for (int c = 0; c < chapters.Length; c++)
        {
            ChapterData chapterData = SaveManager.GetChapterInfo(c, cityNum);
            //int levelsOpen = levelPos.ChapterNum == chapterData.chapterNum ? levelPos.levelNum : chapterData.peoplesCollected.Length;
            //Debug.Log("c: " + c + " levelPos.ChapterNum: " + levelPos.ChapterNum);
            int[] peoplesCollected = chapterData.peoplesCollected;
            int peopleTotal = peoplesCollected.Length * SaveManager.peopleMax;
            int peopleCollect = 0;
            for (int i = 0; i < peoplesCollected.Length; i++)
                peopleCollect += peoplesCollected[i];
            totalChapterPeoples += peopleTotal;
            collectChapterPeoples += peopleCollect;
            chapters[c].setPeopleText(peopleTotal, peopleCollect);
            chapters[c].InitButtons(chapterData, levelPos);
            if(c > 0)//Вторую и третью главы
            {
                float moveToY = distChaptersNorm;
                if (cityNum == 0)//двигаем вверх
                {
                    moveToY = c == 1 ? distChaptersMin : distChaptersNorm;
                }
                Vector3 vectorToY = new Vector3(0, moveToY, 0);
                RectTransform currChapterTR = chapters[c].GetComponent<RectTransform>();
                RectTransform prevChapterTR = chapters[c - 1].GetComponent<RectTransform>();
                currChapterTR.localPosition = prevChapterTR.localPosition - vectorToY;
            }

        }

        chapPassInf.text = collectChapterPeoples.ToString() + "/" + totalChapterPeoples.ToString();

        contentTR.sizeDelta = new Vector2(contentTR.sizeDelta.x, cityNum == 0 ? contentMin : contentMax);
    }

}
