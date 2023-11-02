using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chapter : MonoBehaviour
{
    public Transform buttonsParent, railsParrent;
    public Text peopleText;
    public int myNum;
    private bool startFromLeft;
    private float peopleImageWidth;
    private static System.Random random;

    private void OnEnable()
    {
        startFromLeft = true;
        peopleImageWidth = 44;
        random = new System.Random();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setPeopleText(int peopleTotal, int peopleCollect)
    {

        peopleText.text = peopleCollect.ToString() + "/" + peopleTotal.ToString();
        //Debug.Log("peopleText.text: " + peopleText.text);
    }

    public void InitButtons(ChapterData chapterData, PlayLevelPos levelPlay)
    {
        //setPeopleText(chapterData.peoplesCollected);
        //EventManager.Invoke("PlayLevel", new BecomeEvent(false, 0, 0));
        int[] peoples_ = chapterData.peoplesCollected;
        //int levelPlay = -1;
        int maxLevels = -1;

        if (chapterData.chapterNum == levelPlay.ChapterNum && chapterData.isOpen)
        {
            maxLevels = levelPlay.levelNum;
        }
        else if(chapterData.chapterNum < levelPlay.ChapterNum)
        {
            maxLevels = chapterData.peoplesCollected.Length - 1;
        }

        int countB = -1;
        for (int b = 0; b < buttonsParent.childCount / 4; b++)
        {
            if (startFromLeft)
                for (int currB = 0; currB < 4; currB++)
                {
                    ++countB;
                    int nextButtonInt = b * 4 + currB;
                    int peps = countB < peoples_.Length ? peoples_[countB] : 0;
                    Transform bts = buttonsParent.GetChild(nextButtonInt);
                    buttonSet(maxLevels, countB, nextButtonInt, bts, peoples_.Length, peps);
                }
            else
                for (int currB = 3; currB >= 0; currB--)
                {
                    ++countB;
                    int nextButtonInt = b * 4 + currB;
                    int peps = countB < peoples_.Length ? peoples_[countB] : 0;
                    Transform bts = buttonsParent.GetChild(nextButtonInt);
                    buttonSet(maxLevels, countB, nextButtonInt, bts, peoples_.Length,peps);
                    
                }
            startFromLeft = !startFromLeft;

        }
    }

    private void buttonSet(int levelsOpen, int countB, int nextButtonInt, Transform buttonTR, int peoplesArrLength, int peopleCollect)
    {
        int ppc = -1;
        if (countB < peoplesArrLength)
            ppc = peopleCollect;
        ConfigButton(countB, ppc, levelsOpen, buttonTR);

        if (countB - 1 >= levelsOpen)
        {
            railsParrent.GetChild(countB).gameObject.SetActive(false);
        }
        else
            railsParrent.GetChild(countB).gameObject.SetActive(true);
    }


    private void ConfigButton(int buttonNum_, int peopleColl, int levelsOpen, Transform buttonTR_)
    {
        int condition = 0;
        if (buttonNum_ <= levelsOpen)
        {
            condition = 2;
            if (buttonNum_ == levelsOpen)
            {
                LevelScreen.levelScreen.playImage.transform.parent = buttonTR_;
                LevelScreen.levelScreen.playImage.localPosition = Vector3.zero;
            }
        }
        if (buttonNum_ == 0 && levelsOpen != -1)//Movie play
        {
            //condition = 1;
        }
        if (peopleColl == -1)
            condition = -1;

        Button curButton = buttonTR_.GetComponent<Button>();
        Text buttonText = buttonTR_.GetChild(0).GetComponent<Text>();
        buttonText.text = (buttonNum_ + 1).ToString();
        Image peopleImage = buttonTR_.GetChild(1).GetComponent<Image>();
        Vector2 sizeImage = peopleImage.rectTransform.sizeDelta;
        sizeImage.x = peopleImageWidth * peopleColl;
        peopleImage.rectTransform.sizeDelta = sizeImage;
        SpriteState spST = curButton.spriteState;
        switch (condition)
        {
            case -1://Disable
                curButton.gameObject.SetActive(false);
                break;

            case 0://Close
                curButton.gameObject.SetActive(true);
                float rotateDeg = random.Next(0, 360);
                buttonTR_.Rotate(0, 0, rotateDeg);
                buttonText.transform.Rotate(0, 0, -rotateDeg);
                curButton.image.sprite = LevelScreen.levelScreen. closeBut;
                curButton.interactable = false;
                sizeImage.x = 0;
                peopleImage.rectTransform.sizeDelta = sizeImage;
                break;
            case 1://Play

                curButton.gameObject.SetActive(true);
                curButton.image.sprite = LevelScreen.levelScreen.playingBut;
                spST.pressedSprite = LevelScreen.levelScreen.playingPres;
                curButton.spriteState = spST;
                curButton.onClick.RemoveAllListeners();
                curButton.onClick.AddListener(() => GameplayController.controller.PlayLevel(myNum, buttonNum_));
                buttonText.text = "";
                curButton.interactable = true;
                buttonTR_.rotation = Quaternion.identity;
                buttonText.transform.rotation = Quaternion.identity;
                curButton.image.SetNativeSize();
                //sizeImage.x = 0;
                //peopleImage.rectTransform.sizeDelta = sizeImage;
                break;
            case 2://Complete
                curButton.gameObject.SetActive(true);
                curButton.interactable = true;
                buttonTR_.rotation = Quaternion.identity;
                buttonText.transform.rotation = Quaternion.identity;
                if (peopleColl < SaveManager.peopleMax)
                {
                    curButton.image.sprite = LevelScreen.levelScreen.notComplete;
                    spST.pressedSprite = LevelScreen.levelScreen.notCompletePres;
                }
                else
                {
                    curButton.image.sprite = LevelScreen.levelScreen.completeBut;
                    spST.pressedSprite = LevelScreen.levelScreen.completePres;
                }
                curButton.onClick.RemoveAllListeners();
                curButton.onClick.AddListener(() => GameplayController.controller.PlayLevel(myNum, buttonNum_));

                curButton.spriteState = spST;
                break;
        }


    }
}
