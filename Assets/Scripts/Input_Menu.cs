using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Input_Menu : MonoBehaviour
{
    public Image mainBackground;
    public LevelScreen levelScreen;
    public Canvas PoliticCanvas, LoadingCanvas;
    private AudioSource soundPlayer;
    public Text main_play, main_choose_level, settings_title, settings_sound, settings_music, settings_vibro, settings_policy, settings_language;
    private const float defaultRatio = 1920f / 1080f;
    public string settings_on { get; private set; }
    public string settings_off { get; private set; }

    private void Awake()
    {
        Localizator.Init();
    }

    private void OnEnable()
    {
        Localizator.langDelegate += ChangeLanguage;
        PoliticCanvas.GetComponent<Politic>().OnPoliticClose += RefreshPolitic;
        EventManager.AddListener("PlayLevel", IsLevelPlaying);
        EventManager.AddListener("SettingChange", IsSettingChange);

    }

    private void OnDisable()
    {
        Localizator.langDelegate -= ChangeLanguage;
        PoliticCanvas.GetComponent<Politic>().OnPoliticClose -= RefreshPolitic;
        EventManager.RemoveListener("PlayLevel", IsLevelPlaying);
        EventManager.RemoveListener("SettingChange", IsSettingChange);

    }
    private void IsSettingChange(BecomeEvent BE)
    {
        CheckSound(BE.come);
    }
    private void IsLevelPlaying(BecomeEvent BE)
    {
        CheckSound(!BE.come);
        if(BE.come)
            DeactiveChilds(true);//Deactive Canvases

    }
    // Start is called before the first frame update
    void Start()
    {
        GameplayController.controller.startMovie = new GameplayController.OnMovieStart(DeactiveChilds);
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = true;
        CheckPolitic();
        CheckRatio();
        SaveManager.LoadProgress();
        GameplayController.controller.DeactiveVisibleElements();
        soundPlayer = GetComponent<AudioSource>();
        CheckSound(true);
        Localizator.Start();
    }

    private void CheckSound(bool play_)
    {
        if (soundPlayer)
        {
            if (PlayerPrefs.GetInt("Music") > 0 && play_)
            {
                if(!soundPlayer.isPlaying)
                    soundPlayer.Play();
            }
            else
            {

                soundPlayer.Stop();

            }
        }
    }

    private void CheckPolitic()
    {
        //string[] settings = FileManager.fileManager.GetSettingStrs();
        int politicAccepted = PlayerPrefs.GetInt("politic");

        /*
        if(settings!=null && settings[0].Contains("politic"))
        {
            int num_ = settings[0].IndexOf('=');
            politicAccepted = (int)char.GetNumericValue(settings[0][num_ + 1]);
        }*/

        if (politicAccepted == 0)
        {
            PoliticCanvas.GetComponent<Politic>().OpenFrom(false);
            //DeactiveChilds(true);
            //GetComponent<Canvas>().enabled = false;
        }
        
    }

    private void ChangeLanguage()
    {
        //Debug.Log("Lang change");
        main_choose_level.text = Localizator.GetLangChangeItem("main_choose_level");
        main_play.text = Localizator.GetLangChangeItem("main_play");
        settings_title.text = Localizator.GetLangChangeItem("settings_title"); 
        settings_sound.text = Localizator.GetLangChangeItem("settings_sound"); 
        settings_music.text = Localizator.GetLangChangeItem("settings_music");
        settings_vibro.text = Localizator.GetLangChangeItem("settings_vibro");
        settings_policy.text = Localizator.GetLangChangeItem("settings_policy");
        settings_language.text = Localizator.GetLangChangeItem("settings_language");
        settings_off = Localizator.GetLangChangeItem("settings_off");
        settings_on = Localizator.GetLangChangeItem("settings_on");
    }

    private void RefreshPolitic(int newVar_)
    {
        PlayerPrefs.SetInt("politic", newVar_);
        PlayerPrefs.Save();
        if(newVar_ == 1)
        {
            PoliticCanvas.enabled = false;
            //GetComponent<Canvas>().enabled = true;
        }
    }

    private void CheckRatio()
    {
        float ratio = (float)Screen.height / (float)Screen.width;

        /*if(ratio > defaultRatio)
        {
            float multipleSize = ratio / defaultRatio;
            Debug.Log("multipleSize *default: " + multipleSize);
            mainBackground.rectTransform.sizeDelta *= multipleSize;
        }
        else*/ if(ratio < defaultRatio)
        {
            float multipleSize = defaultRatio / ratio;
            Debug.Log("multipleSize *altRatio: " + multipleSize);
            mainBackground.rectTransform.sizeDelta *= multipleSize;
        }
    }

    private void DeactiveChilds(bool isDeactivate)
    {
        CheckSound(false);
        //Debug.Log("Deactive menu elems: " + isDeactivate);
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(!isDeactivate);
        }
    }


    public void OpenLevelScreen()
    {
        DeactiveChilds(false);
        levelScreen.gameObject.SetActive(true);
        levelScreen.ReInitButtons();
        EventManager.Invoke("TrainStart", new BecomeEvent(false, 0, 0));
    }

    public void ClickStart()
    {

        //Debug.LogError("error!!!");
        GameplayController.controller.StartLevel();
        
      // StartCoroutine(startLoading());
    }

    IEnumerator startLoading()
    {
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(2);
        yield return null;
        
    }

    public void ClickSetting()
    {

    }

}
