using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Politic : MonoBehaviour
{
    public Toggle toggleAccept;
    public Text buttonBeginText;
    public Color textEnable, textDisable;
    public Scrollbar scrollbarVert;
    private bool openFomSetting;
    public delegate void SettingChanged(int newVar_);
    public event SettingChanged OnPoliticClose;
    // Start is called before the first frame update
    void Start()
    {
        if(scrollbarVert)
        {
            scrollbarVert.value = 1f;
        }
    }

    public void TryClose()
    {
        if (!toggleAccept.isOn)//Не приняты условия политики
        {
            OnPoliticClose(0);
            Application.Quit(0);
        }
        else
        {
            OnPoliticClose(1);
            GetComponent<Canvas>().enabled = false;
        }

    }

    public void ButtonBeginEnabled(bool isEnable)
    {

        buttonBeginText.color = isEnable ? textEnable : textDisable;

        
    }

    public void OpenPoliticURL()
    {
        Application.OpenURL("https://www.wonder.legal/ru/modele/%D0%BF%D0%BE%D0%BB%D0%B8%D1%82%D0%B8%D0%BA%D0%B0-%D0%BA%D0%BE%D0%BD%D1%84%D0%B8%D0%B4%D0%B5%D0%BD%D1%86%D0%B8%D0%B0%D0%BB%D1%8C%D0%BD%D0%BE%D1%81%D1%82%D0%B8-%D0%BC%D0%BE%D0%B1%D0%B8%D0%BB%D1%8C%D0%BD%D0%BE%D0%B3%D0%BE-%D0%BF%D1%80%D0%B8%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D1%8F");
    }

    public void OpenURL(bool rules_)
    {
        if(rules_)
        {
            Application.OpenURL("https://ru.wikipedia.org/wiki/%D0%A3%D1%81%D0%BB%D0%BE%D0%B2%D0%B8%D1%8F_%D0%B8%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D1%8F");

        }
        else
        {
            Application.OpenURL("https://ru.wikipedia.org/wiki/%D0%97%D0%B0%D1%89%D0%B8%D1%82%D0%B0_%D0%BF%D0%B5%D1%80%D1%81%D0%BE%D0%BD%D0%B0%D0%BB%D1%8C%D0%BD%D1%8B%D1%85_%D0%B4%D0%B0%D0%BD%D0%BD%D1%8B%D1%85");
        }
    }

    public void OpenFrom(bool settingMenu_)
    {
        openFomSetting = settingMenu_;
        if (settingMenu_)
        {
            toggleAccept.interactable = false;
            toggleAccept.isOn = true;
        }
        else
        {
            toggleAccept.isOn = false;
            toggleAccept.interactable = true;
        }
            GetComponent<Canvas>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
