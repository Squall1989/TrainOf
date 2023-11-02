using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    public Transform GearTS, TrainTS, PlanetTS;
    public float trainSpeed, gearSpeed, planetSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = true;
        SetPlayerPrefs();
        if(PlayerPrefs.GetInt("Music") == 0)
        {
            GetComponent<AudioSource>().Stop();
        }
        else
            GetComponent<AudioSource>().Play();


        StartCoroutine(LoadTimer());
    }

    private void SetPlayerPrefs()
    {
        if(PlayerPrefs.GetInt("Setting") != 1)
        {
            PlayerPrefs.SetInt("Sound", 1);

            PlayerPrefs.SetInt("Music", 1);
            Debug.Log("Music and sound set 1");
        }
    }

    IEnumerator LoadTimer()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadSceneAsync(1);
    }

    // Update is called once per frame
    void Update()
    {
        GearTS.Rotate(0, 0, -gearSpeed);
        TrainTS.Rotate(0, 0, trainSpeed);
        PlanetTS.Rotate(0, 0, -planetSpeed);
    }
}
