using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Education : MonoBehaviour
{
    public Transform hand, line, circle;
    private Tail movingTale;
    private Vector3 targetPos;
    private const float moveSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playEducation());
    }

    private void UpDown(float heightTo)
    {
        movingTale.transform.position += movingTale.transform.up * heightTo;
    }
    
    private void RightLeft(float rightTo)
    {
        Vector3 newPos = (targetPos - movingTale.transform.position).normalized;
        newPos *= rightTo;
        movingTale.transform.position += newPos;
    }

    IEnumerator playEducation()
    {
        const float upTale = .1f;
        movingTale = getTale(2, 1);
        hand.position = Camera.main.WorldToScreenPoint(movingTale.transform.position);
        targetPos = getTale(1, 1).transform.position + getTale(1, 1).transform.up * upTale;
        line.position = hand.position;
        RectTransform lineRect = line.GetComponent<RectTransform>();
        lineRect.sizeDelta = new Vector2(0, lineRect.sizeDelta.y);
        circle.position = hand.position;

        circle.GetChild(0).gameObject.SetActive(false);
        circle.gameObject.SetActive(false);
        yield return new WaitForSeconds(.6f);
        circle.gameObject.SetActive(true);
        yield return new WaitForSeconds(.2f);
        circle.GetChild(0).gameObject.SetActive(true);
        UpDown(upTale);

        ControlTales.controlTales.FalseTaleMoveStart(movingTale, hand.position);

        yield return new WaitForSeconds(1f);

        while ((movingTale.transform.position -targetPos).magnitude >= .01f)
        {
            RightLeft(Time.deltaTime * moveSpeed);

            hand.position = Camera.main.WorldToScreenPoint(movingTale.transform.position);
            circle.position = hand.position;
            lineRect.sizeDelta = new Vector2((line.position - hand.position).magnitude, lineRect.sizeDelta.y);
            yield return null;
        }
        
        yield return new WaitForSeconds(.4f);
        circle.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(.2f);
        circle.gameObject.SetActive(false);
        UpDown(-upTale);
        lineRect.sizeDelta = new Vector2(0, lineRect.sizeDelta.y);
        yield return new WaitForSeconds(1.4f);

        ControlTales.controlTales.FalseMoveEnd();
        PlayerPrefs.SetInt("Education", 1);
        PlayerPrefs.Save();
        GameplayController.controller.StartLevel();
        gameObject.SetActive(false);
    }

    private Tail getTale(int x, int z)
    {
        PositionTale pos1;
        pos1.X = x;
        pos1.Z = z;
        return TailsTable.talesTable.GetTale(pos1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
