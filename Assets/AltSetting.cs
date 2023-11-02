using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltSetting : MonoBehaviour
{
    private const float speedCorrect = 1 / 20f;
    private const float minSpeed = 20f, maxSpeed = 100f;
    public Text normSpeed, collectSPeed;
    public Scrollbar normBar, slowBar;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        float spdNorm = PlayerPrefs.GetFloat("trainSpeed");
        float spdLow = PlayerPrefs.GetFloat("lowSpeed");
        Debug.Log("Get norm speed: " + spdNorm);
        Debug.Log("Get low speed: " + spdLow);
        if (spdNorm == 0)
        {
            SetNormSpeed(.3f);
            normBar.value = .3f;
        }
        else
        {
            normSpeed.text = spdNorm.ToString() + " км/ч";
            normBar.value = (spdNorm - minSpeed) / (maxSpeed - minSpeed);
        }
        if (spdLow == 0)
        {
            SetLowSpeed(.1f);
            slowBar.value = .1f;
        }
        else
        {
            collectSPeed.text = spdLow.ToString() + " км/ч";
            slowBar.value = (spdLow - minSpeed) / (maxSpeed - minSpeed);
        }
    }

    

    public void SetNormSpeed(float value_)
    {
        float speed_ = minSpeed + (maxSpeed - minSpeed) * value_;
        PlayerPrefs.SetFloat("trainSpeed", speed_);
        Debug.Log("Set norm speed: " + speed_);
        normSpeed.text = ((int)speed_).ToString() + " км/ч";
    }

    public void SetLowSpeed(float value_)
    {
        float speed_ = minSpeed + (maxSpeed - minSpeed) * value_;
        PlayerPrefs.SetFloat("lowSpeed", speed_);
        Debug.Log("Set low speed: " + speed_);
        collectSPeed.text = ((int)speed_).ToString() + " км/ч";
    }
}
