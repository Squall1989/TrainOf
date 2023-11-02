using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mechanics : MonoBehaviour
{
    public static Mechanics mechanics;
    
    public Text stepsTXT, timerTXT;
    public Image stepImage, timeImage, healImage;
    public Sprite StepsRed, StepsYellow, StepsGreen;
    public Sprite TimeRed, TimeYellow, TimeGreen;
    public Sprite[] Heal;

    private int steps, heals, startSteps;
    private TimeMechanic timer;
    private bool stepEnable, timeEnable, healEnable;

    private void Awake()
    {
        mechanics = this;

    }

    private void OnEnable()
    {
        EventManager.AddListener("PlayLevel", LevelStart);
        EventManager.AddListener("TrainStart", TrainStarted);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener("PlayLevel", LevelStart);
        EventManager.RemoveListener("TrainStart", TrainStarted);

    }

    public bool IsTimeMechanic()
    {
        return timeEnable;
    }

    public void SetMechanics(string[] mechstr)
    {
        StopAllCoroutines();
        for (int i = 1; i < mechstr.Length; i++)
        {
            changeMechanic(new MechanicSet(i, mechstr[i]));
        }

        if (timer.min == 0 && timer.sec == 0)
        {
            timeEnable = false;
        }
        else
        {
            timeEnable = true;
            timerTXT.text = timer.GetStrTime();
            StartCoroutine(timeToTrainStart());
        }


        stepImage.enabled = stepEnable;
        stepImage.sprite = StepsGreen;
        stepsTXT.enabled = stepEnable;

        timeImage.enabled = timeEnable;
        timerTXT.enabled = timeEnable;

        healImage.enabled = healEnable;
        if (healEnable)
        {
            heals = 3;
            healImage.sprite = Heal[heals];
        }
    }

    private void TrainStarted(BecomeEvent BE)
    {
        if(BE.come)
            StopAllCoroutines();
        
    }

    private IEnumerator timeToTrainStart()
    {
        while(timer.min > 0 || timer.sec > 0)
        {
            yield return new WaitForSeconds(1f);
            timer.MinusTime(1f);
            timerTXT.text = timer.GetStrTime();

            float percent = timer.GetPercent();
            if(percent < .33f)
            {
                timeImage.sprite = TimeRed;
            }
            else if(percent < .66f)
            {
                timeImage.sprite = TimeYellow;
            }
        }
        if(timer.min <= 0 && timer.sec <= 0)
        {
            GameplayController.controller.TrainStartTimer();
        }
    }

    private void LevelStart(BecomeEvent BE)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(BE.come);
        }
    }

    public void HealMinus()
    {
        if(healEnable)
        {
        
            if(--heals <= 0)
            {
                GameplayController.controller.TrainOverDamage();
                //Train stop
                //Train finish
            }
            if (heals >= 0)
                healImage.sprite = Heal[heals];

        }

    }

    public void StepMinus()
    {
        if(stepEnable)
        {
            steps--;
            if(steps >= 0)
                stepsTXT.text = steps.ToString();

            if (steps <= 0)
            {
                StartCoroutine(WaitForPhysicsUpdate());
                //End
            }
            else
            {
                float percent = (float)steps / (float)startSteps;
                if (percent < .33f)
                {
                    stepImage.sprite = StepsRed;
                }
                else if (percent < .66f)
                {
                    stepImage.sprite = StepsYellow;
                }
            }
        }
    }

    private IEnumerator WaitForPhysicsUpdate()
    {
        yield return new WaitForSeconds(.05f) ;
        GameplayController.controller.StepsEnded();

    }

    private void changeMechanic(MechanicSet mechanicSet)
    {
        if (mechanicSet.value == "")
            mechanicSet.value = "0";
        switch (mechanicSet.num)
        {
            case 1:
                stepsTXT.text = mechanicSet.value;
                steps = System.Convert.ToInt32(mechanicSet.value);
                startSteps = steps;
                stepEnable = steps != 0;
                break;
            case 2:
                //timerTXT.text = mechanicSet.value;
                timer.setMin(System.Convert.ToInt32(mechanicSet.value));
                if (timer.min > 0)
                    timeImage.sprite = TimeGreen;
                break;
            case 3:
                timer.setSec(System.Convert.ToInt32(mechanicSet.value));
                //if (mechanicSet.value != "0" && mechanicSet.value != "00")
                if(timer.sec > 0)
                    timeImage.sprite = TimeGreen;
                break;
            case 4:
                healEnable = (mechanicSet.value == "True");
                heals = healEnable ? 3 : 0;
                break;
        }
        
    }
}
public struct MechanicSet
{
    public int num;
    public string value;
    public MechanicSet(int num_, string value_)
    {
        num = num_;
        value = value_;
    }
}
public struct TimeMechanic
{
    public int min;
    public float sec;
    private int startMin;
    private int startSec;
    public void MinusTime(float delta)
    {
        sec -= delta;
        if(sec < 0)
        {
            if (min > 0)
            {
                float minusSec = sec;
                sec = 60f;
                sec += minusSec;
                min--;
            }
            else
                sec = 0;
        }
    }

    public float GetPercent()
    {
        float startTime = startMin * 60f + startSec;
        float currTime = min * 60f + sec;
        return currTime / startTime;
    }

    public void setSec(int sec_)
    {
        sec = sec_;
        startSec = sec_;
    }

    public void setMin(int min_)
    {
        min = min_;
        startMin = min_;
    }
    public string GetStrTime()
    {
        string minStr = min < 10 ? "0" + min.ToString() : min.ToString();
        string secStr = sec < 10 ? "0" + sec.ToString() : sec.ToString();
        return minStr + ":" + secStr;
    }
}
