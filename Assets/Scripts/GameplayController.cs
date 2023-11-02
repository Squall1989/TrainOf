using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayController : MonoBehaviour
{
    private Camera mainCam;
    public static GameplayController controller;
    public Education education;
    public Transform WinFailPanel, winElements, failElements, bottomButtons, LevelInfo, buttonNext;
    public Text passengerStatus, levelNumText, chapterNameText, chapterNumText;
    public TextMeshProUGUI percentText;
    public Transform percentImagesParent;
    public delegate void OnMovieStart(bool deactive);
    public AudioClip winSound;
    public OnMovieStart startMovie;
    private StartTail startTail1, startTail2;
    private GameObject[] percentImages = new GameObject[10];
    private bool isLevelPlay = false;
    private const string chapterGetStr = "chapter_name_";

    private AudioSource playerAudio;
    int passengers;
    private PlayLevelPos currLevel;


    private void Awake()
    {
        controller = this;
        passengerStatus.text = "0/3";
        currLevel = new PlayLevelPos(-1, 0, 0);
        playerAudio = GetComponent<AudioSource>();
        for(int i = 0; i < percentImages.Length; i++)
        {
            percentImages[i] = percentImagesParent.GetChild(i).gameObject;
        }
    }
    private void OnEnable()
    {
        passengers = 0;
        SetActiveElements(false);
        mainCam = Camera.main;
        Localizator.langDelegate += ChangeLanguage;
    }
    private void OnDisable()
    {
        Localizator.langDelegate -= ChangeLanguage;
    }

    public PlayLevelPos getLevel()
    {
        return currLevel;
    }
    public void SetStartTail(StartTail tail_)
    {
        if (startTail1)
        {
            startTail2 = tail_;
        }
        else
        {
            startTail1 = tail_;
        }
    }

    private void ClearStartTails()
    {
        startTail1 = null;
        startTail2 = null;
    }

    public void PlayGameplaySound(AudioClip clip_)
    {
        if(PlayerPrefs.GetInt("Sound") > 0)
            playerAudio.PlayOneShot(clip_);
    }

    public void PlayGameplayMusic(AudioClip clip_)
    {
        if (PlayerPrefs.GetInt("Music") > 0)
            playerAudio.PlayOneShot(clip_);
    }

    public bool IsLevelPlaying()
    {
        return isLevelPlay;
    }
    /*
    public void SetPlayingCity(int num_)
    {
        playCity = num_;
    }
    */
    public void PlayLevel(int chapterNum_, int levelNum_)
    {
        currLevel.ChapterNum = chapterNum_;
        currLevel.levelNum = levelNum_;
        //currLevel.cityNum = LevelScreen.levelScreen.GetCity().CityNum;


        if (currLevel.cityNum == 0 && currLevel.ChapterNum == 0 
            && currLevel.levelNum == 0 && PlayerPrefs.GetInt("MovieViewed") == 0)
        {
            if (startMovie != null)
                startMovie.Invoke(true);
            else
                Debug.Log("Not set delegate");
            Debug.Log("Play movie");
            PlayMovie(1);
            
            PlayerPrefs.SetInt("MovieViewed", 1);
        }
        else
        {
            StartLevel();
        }
    }

    public void TrainOverDamage()
    {
        if (startTail1)//ToDo
            startTail1.StopTrain();
        TrainFinish(false);
    }

    public void TrainStartTimer()
    {
        if(startTail1)//ToDo
        {
            if (!startTail1.StartTrainFromTimer())
                TrainFinish(false);
        }
    }

    private void ChangeLanguage()
    {
        if (currLevel.levelNum == -1)
        {
            currLevel = SaveManager.GetStartLevel();
        }
        chapterNameText.text = Localizator.GetLangChangeItem(chapterGetStr + (currLevel.ChapterNum+1).ToString());
    }

    public void StartLevel()
    {

        if(currLevel.levelNum == -1)
        {
            currLevel = SaveManager.GetStartLevel();
        }
        ClearStartTails();

        EventManager.Invoke("PlayLevel", new BecomeEvent(true, currLevel.ChapterNum, currLevel.levelNum));
        isLevelPlay = true;
        passengers = 0;
        passengerStatus.text = "0/3";
        levelNumText.text = (currLevel.levelNum + 1).ToString();
        chapterNumText.text = (currLevel.ChapterNum + 1).ToString();
        ChangeLanguage();
        LevelInfo.gameObject.SetActive(true);

        mainCam.gameObject.SetActive(true);
        FileManager.LoadFile(currLevel.levelNum +1, currLevel.ChapterNum +1);
        SetActiveElements(true);
        if (SaveManager.GetCityInfo(0)[0].peoplesCollected[0] == 0 && PlayerPrefs.GetInt("Education") == 0)
        {
            education.gameObject.SetActive(true);
        }
        ControlTales.controlTales.setRotZero();
    }

    private void PlayMovie(int num_)
    {
        LevelInfo.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(false);
        Movie.movie.PlayAnim(num_);
    }
    
    public void RestartLevel()
    {
        EventManager.Invoke("TrainStart", new BecomeEvent(false, 0, 0));

        PlayLevel(currLevel.ChapterNum, currLevel.levelNum);
    }

    public void DeactiveVisibleElements()
    {
        WinFailPanel.gameObject.SetActive(false);
        bottomButtons.gameObject.SetActive(false);
    }

    public void PassengerPlus()
    {
        passengerStatus.text = ++passengers + "/3";
    }

    public void StepsEnded()
    {
        if (startTail1)
            if (!startTail1.CheckAllConnections())// && (startTail2 && !startTail2.CheckAllConnections()))
            {
                Debug.Log("startTail1: " + startTail1.name);
                TrainFinish(false);
            }
    }

    public void TrainFinish(bool onFinishTail)
    {
        isLevelPlay = false; 

        SetActiveElements(false);
        bool win = passengers > 0 && onFinishTail;
        if (win)
            SaveManager.SaveProgress(currLevel, passengers);

        winElements.gameObject.SetActive(win);
        failElements.gameObject.SetActive(!win);
        buttonNext.gameObject.SetActive(win);
        if(win)
        {
            PlayGameplayMusic(winSound);
        }
        int percentProgress = SaveManager.GetChapterProgress(currLevel);//(int)(100f * ((float)passengers / (float)SaveManager.peopleMax));
        int progressImagesEnable = percentProgress / 10;
        for(int i = 0; i < percentImages.Length; i++)
        {
            percentImages[i].SetActive(i <= progressImagesEnable);
        }
        
        //if (!win)
          //  percentProgress = 0;
        percentText.text = percentProgress.ToString() + "%";
        SetNextLevel();

    }

    private void SetActiveElements(bool start_)
    {
        WinFailPanel.gameObject.SetActive(!start_);
        bottomButtons.gameObject.SetActive(start_);

    }
    
    private void SetNextLevel()
    {
        passengers = -1;
        PassengerPlus();
        currLevel = SaveManager.NextLevel(currLevel);
    }

    public void NextLevel()
    {
        SetActiveElements(true);
        
        PlayLevel(currLevel.ChapterNum, currLevel.levelNum);
        EventManager.Invoke("TrainStart", new BecomeEvent(false, 0, 0));
    }

}
