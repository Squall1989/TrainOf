using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityStatus : MonoBehaviour
{
    public static CityStatus statusMap;

    private const int cityCount = 5;
    
    public enum StatusCity { Close, Play, Complete};
    public StatusCity statusCity { get; private set; }

    public Text[] peopleText;//, levelPeopleText;
    public LevelScreen levelScreen;

    public Transform parentPoses;
    private Transform[] wagonPositions;
    public RectTransform[] wagons;

    private void Awake()
    {
        statusMap = this;
    }

    void Start()
    {
        wagonPositions = new RectTransform[parentPoses.childCount];
        for (int i = 0; i < parentPoses.childCount; i++)
        {
            wagonPositions[i] = parentPoses.GetChild(i);
        }
        statusCity = StatusCity.Play;//ToDo
    }

    private void OnEnable()
    {
        LoadCityData();

    }

    public void LoadCityData()
    {
        for (int s = 0; s < cityCount; s++)
        {
            ChapterData[] cityData = SaveManager.GetCityInfo(s);
            if (!cityData[0].isOpen)
                break;

            int peopleMax = 0;
            int peopleCollected = 0;
            for (int c = 0; c < cityData.Length; c++)
            {
                peopleMax += cityData[c].peoplesCollected.Length * SaveManager.peopleMax;
                for (int p = 0; p < cityData[c].peoplesCollected.Length; p++)
                {
                    peopleCollected += cityData[c].peoplesCollected[p];

                }
            }
            peopleText[s].text = peopleCollected.ToString() + "/" + peopleMax.ToString();
        }
    }

    public void clickCity(int num_)
    {
        //LoadCityData();
        //if (statusCity == StatusCity.Play)
        {

            //levelPeopleText.text = peopleText.text;
            EventManager.Invoke("PlayLevel", new BecomeEvent(false,0,0));
            levelScreen.gameObject.SetActive(true);
            levelScreen.InitChapters(num_);
        }
       // else
           // Debug.Log("City not active");
    }

}
