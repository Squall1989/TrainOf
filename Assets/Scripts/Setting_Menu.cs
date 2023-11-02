using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting_Menu : MonoBehaviour
{
    public Input_Menu menu;
    public AudioClip changeTumbSound, buttonTapSound, openPanelSound, closePanelSound;
    private AudioSource soundPlayer;
    public Scrollbar[] scrollbars;
    public Text[] scrollTexts;
    public Sprite offSprite, onSprite;
    public Color colorOn, colorOff;
    //private Scrollbar changedBar;
    //private Text changedText;
    private Image backImage;
    private bool isHandlePointer;
    private int thumblerNum;
    private int[] thumblerLastInt;


    private void OnEnable()
    {
        Localizator.langDelegate += ChangeLang;

    }

    private void OnDisable()
    {
        Localizator.langDelegate -= ChangeLang;

    }

    // Start is called before the first frame update
    void Start()
    {
        thumblerLastInt = new int[scrollbars.Length];
        ChangeLang();
        soundPlayer = GetComponent<AudioSource>();
        backImage = GetComponent<Image>();
        LoadSetting();
    }

    private void ChangeLang()
    {
        string on = Localizator.GetLangChangeItem("settings_on");
        string off = Localizator.GetLangChangeItem("settings_off");
        for (int i = 0; i < scrollbars.Length; i++)
        {
            Scrollbar scrollbar = scrollbars[i];
            Text scrollText = scrollTexts[i];
            
            scrollText.text = scrollbar.value == 0 ? off : on;
        }
    }
    private void LoadSetting()
    {
        SetThumblerNum(0);
        int soundTurn = PlayerPrefs.GetInt("Sound");
        SetThumblerFromStart(soundTurn);

        SetThumblerNum(1);
        int musicTurn = PlayerPrefs.GetInt("Music");
        SetThumblerFromStart(musicTurn);

        SetThumblerNum(2);
        int vibrateTurn = PlayerPrefs.GetInt("Vibrate");
        SetThumblerFromStart(vibrateTurn);
    }

    private void SetThumblerFromStart(int setInt_)
    {
        //changedBar.value = setInt_;
        ThumblerTurn(setInt_);
    }

    public void LangClick(bool open_)
    {
        if(PlayerPrefs.GetInt("Sound") > 0)
        {
            soundPlayer.PlayOneShot(buttonTapSound);
        }
    }

    public void ChangeThumbler(float value)
    {
        int turnInt = thumblerLastInt[thumblerNum] == 0 ? 1 : 0;

        ThumblerTurn(turnInt);

    }

    private IEnumerator blockThumbler(Scrollbar thumbler)
    {
        thumbler.interactable = false;

        yield return new WaitForSeconds(.5f);

        thumbler.interactable = true;
    }
    /* 
    public void ClickThumbler()
    {
        Debug.Log("Click thumbler");
        changedBar.value = changedBar.value == 0 ? 1 : 0;
        ChangeThumbler(changedBar.value);
       

        if (PlayerPrefs.GetInt("Sound") > 0)
        {
            soundPlayer.PlayOneShot(changeTumbSound);
        }
    }
    */

    
    private void ThumblerTurn(int turn_int)
    {
        if (!scrollbars[thumblerNum].interactable)
            return;

        scrollbars[thumblerNum].GetComponent<Image>().sprite = turn_int == 1 ? onSprite : offSprite;
        scrollbars[thumblerNum].value = turn_int;
        scrollTexts[thumblerNum].text = turn_int == 1 ? menu.settings_on : menu.settings_off;
        scrollTexts[thumblerNum].color = turn_int == 1 ? colorOn : colorOff;
        scrollTexts[thumblerNum].alignment = turn_int == 1 ? TextAnchor.UpperLeft : TextAnchor.MiddleRight;

        

        if (thumblerNum == 2 && turn_int == 1)
        {
            Debug.Log("Vibrate");
#if UNITY_ANDROID
            if (backImage.isActiveAndEnabled)
                Handheld.Vibrate();
#endif
        }

        StartCoroutine(blockThumbler(scrollbars[thumblerNum]));
        SaveThumblerSetting(turn_int);
        thumblerLastInt[thumblerNum] = turn_int;
    }

    private void SaveThumblerSetting(int setInt_)
    {
        switch(thumblerNum)
        {
            case 0:
            PlayerPrefs.SetInt("Sound", setInt_);
                break;
            case 1:
                PlayerPrefs.SetInt("Music", setInt_);

                break;
            case 2:
                PlayerPrefs.SetInt("Vibrate", setInt_);
                break;
        }
        if(PlayerPrefs.GetInt("Sound") > 0 && backImage.isActiveAndEnabled)
        {
            soundPlayer.PlayOneShot(changeTumbSound);
        }
        EventManager.Invoke("SettingChange", new BecomeEvent(true, 0, 0));
    }

    public void PointerOnHandle(bool isOver)
    {
        isHandlePointer = isOver;
    }

    public void OpenPanel()
    {
        PlayerPrefs.SetInt("Setting", 1);

        bool isOpened = backImage.isActiveAndEnabled;

        if(PlayerPrefs.GetInt("Sound") > 0)
        {
            if (isOpened)
                soundPlayer.PlayOneShot(closePanelSound);
            else
                soundPlayer.PlayOneShot(openPanelSound);
        }

        backImage.enabled = !isOpened;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(!isOpened);
        }
    }

    public void SetThumblerNum(int num_)
    {
        //changedText = scrollTexts[num_];
        //isThumblerOn = scrollbars[num_].value == 0 ? false : true;
        thumblerNum = num_;
    }


}
