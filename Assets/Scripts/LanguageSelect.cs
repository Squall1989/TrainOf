using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LanguageSelect : MonoBehaviour, IDeselectHandler
{
    public Setting_Menu setting;
    public GameObject LanguageGameObject;
    public Image langFlag;
    public Text langText;

    public Image currLangFlag;
    public Text currLangText;
    public Sprite[] langFlags;
    private string[] listItems;

    private void OnEnable()
    {
        string langStr = Localizator.GetLangChangeItem("settings_languages");
        string[] langItems = langStr.TrimStart('[').TrimEnd(']').Trim().Split(',');
        ChangeItems(langItems);
    }

    private void ChangeItems(string[] items)
    {
        listItems = items;
        int currLang = PlayerPrefs.GetInt("Language");
        SetFlagText(true, currLang);//ToDo more lang
        SetFlagText(false, currLang == 0 ? 1 : 0);//ToDo

    }

    public void ClickButtonLang()
    {
        LanguageGameObject.SetActive(!LanguageGameObject.activeSelf);
        setting.LangClick(true);//sound
    }

    private void SetFlagText(bool forButton, int num_)
    {
        Text settingText = forButton ? currLangText : langText;
        Image settingFlag = forButton ? currLangFlag : langFlag;
        settingText.text = listItems[num_];
        settingFlag.sprite = langFlags[num_];

        
    }

    public void SelectLanguage(int num_)
    {
        int currLang = PlayerPrefs.GetInt("Language");
        num_ = currLang == 0 ? 1 : 0;//ToDo more langs
        PlayerPrefs.SetInt("Language", num_);
        setting.LangClick(true);//sound

        SetFlagText(true, num_);
        SetFlagText(false, num_ == 0 ? 1 : 0);

        LanguageGameObject.SetActive(false);
        

        Localizator.ChangeLang(num_);
    }

    public void OnDeselect(BaseEventData data)
    {
        //LanguageGameObject.SetActive(false);

    }
}
