using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public float trainSpeed;
    private float rotateSpeed;
    public Transform lokoPoint, tailPoint;
    //private int lokoPos;
    private const float speedCorrect = 1/20f;
    private const float rotateMultiplier = 92f / 2.5f ;
    private float lowSpeed, lowRotate, normSpeed, normRotate;
    private LinkedList<Transform> wayList;
    private List<RailConnector> connectorList;
    private Transform tailTS, lokoTS, lokoChild, tailChild;
    private Coroutine wayCorout;
    private bool OnFinishTail;
    // Start is called before the first frame update
    void Start()
    {
        OnFinishTail = false;
        connectorList = new List<RailConnector>();
        //wayList = new List<Transform>();
        tailTS = transform.GetChild(0);
        lokoTS = transform.GetChild(1);
        lokoChild = lokoTS.GetChild(1);
        tailChild = tailTS.GetChild(1);
        //lokoPos = 0;
        normSpeed = PlayerPrefs.GetFloat("trainSpeed") <= 0 ? trainSpeed/speedCorrect : PlayerPrefs.GetFloat("trainSpeed");
        normRotate = normSpeed * rotateMultiplier;

        
        lowSpeed = PlayerPrefs.GetFloat("lowSpeed") <= 0 ? (trainSpeed/3f)/speedCorrect : PlayerPrefs.GetFloat("lowSpeed");
        lowRotate = lowSpeed * rotateMultiplier;


        //trainSpeed = normSpeed;
        //rotateSpeed = normRotate;
    }

    private void changeSpeed(bool slow)
    {
        if (slow)
        {
            trainSpeed = lowSpeed;
            rotateSpeed = lowRotate;

        }
        else
        {
            trainSpeed = normSpeed;
            rotateSpeed = normRotate;

        }
    }

    public void ComeOnFinish()
    {
        OnFinishTail = true;
    }

    public Transform LokoForPassenger()
    {
        return lokoPoint;
    }
    public Transform TailForPassenger()
    {
        return tailPoint;
    }

    public void Stop()
    {
        StopAllCoroutines();
        trainSpeed = 0;
        rotateSpeed = 0;
    }

    private void ConvertPoints()
    {
        wayList = new LinkedList<Transform>();
        for(int i = 0; i < connectorList.Count; i++)
        {

            for (int w = 0; w < connectorList[i].wayPoints.Length; w++)
            {
                wayList.AddLast(connectorList[i].wayPoints[w]);
            }
        }
    }

    public void StartForWayPoints(List<RailConnector> connectors)
    {
        if (connectorList.Count > 0)
            return;
        for (int c = 0; c < connectors.Count; c++)
            connectorList.Add(connectors[c]);
        ConvertPoints();
        //transform.LookAt(wayList[0]);
        trainSpeed = normSpeed;
        rotateSpeed = normRotate;
        wayCorout = StartCoroutine(WayCoroutine());
    }

    public void AddConnector(RailConnector newConnector)
    {
        connectorList.Add(newConnector);
        for (int w = 0; w < newConnector.wayPoints.Length; w++)
        {
            wayList.AddLast(newConnector.wayPoints[w]);
        }
    }

    public void DisconnectConnector(RailConnector lostConnector)
    {
        //RailConnector currConnector = connectorList[connectorList.Count - 1];
        int connectNum = connectorList.LastIndexOf(lostConnector);
        Debug.Log("disconnect connectNum: " + connectNum);
        if (connectNum == -1)
        {
            Debug.Log("disconnect -1");
            return;
        }
        for (int c = connectorList.Count-1; c > connectNum; c--)
        {
            RailConnector currConnector = connectorList[c];
            for (int i = 0; i < currConnector.wayPoints.Length; i++)
            {
                wayList.RemoveLast();
            }
        }
        connectorList.RemoveRange(connectNum+1,connectorList.Count - connectNum-1);

    }

    private void RotateLokoTail(Transform lokoTail, Transform wayPoint)
    {
        float angle = Vector3.Angle(lokoTail.forward, wayPoint.position - lokoTail.position);
        if(angle != 0)//log == Infinity
            angle = Mathf.Log(angle,5f);
        float inverse = lokoTail.InverseTransformPoint(wayPoint.position).x;
        if (inverse < 0 )
            angle *= -1;
        lokoTail.Rotate(Vector3.up, angle * Time.deltaTime * rotateSpeed * speedCorrect, Space.World);
        
    }

    private int BlockNextRail(int railNum)
    {
        if (railNum < connectorList.Count)
        {
            connectorList[railNum].GetParentTale().BlockTail(true);
            return connectorList[railNum].wayPoints.Length;
        }
        else
            return 0;
    }

    private IEnumerator WayCoroutine()
    {
        EventManager.Invoke("TrainStart", new BecomeEvent(true, 0, 0));
        LinkedListNode<Transform> wayNode = wayList.First;
        transform.LookAt(wayNode.Value.position);
        int railToBlock = 1;
        int wayNextCount = BlockNextRail(railToBlock);
        int currWayCount = 0;
        while (wayNode != null)
        {
            LinkedListNode<Transform> nextNode = wayNode.Next;
            while (nextNode != null)
            {
                float forwardLokoPoint = lokoChild.InverseTransformPoint(nextNode.Value.position).z;
                if (forwardLokoPoint > 0 )
                {
                    RotateLokoTail(lokoTS, nextNode.Value);
                    break;

                }
                nextNode = nextNode.Next;
                
            }

            Vector3 forwardVector = wayNode.Value.position - transform.position;
            transform.position += forwardVector.normalized * Time.deltaTime * trainSpeed * speedCorrect;

            LinkedListNode<Transform> prevNode = wayNode.Previous;
            while (prevNode != null)
            {
                float backwardTailPoint = tailChild.InverseTransformPoint(prevNode.Value.position).z;
                if (backwardTailPoint > 0)
                {
                    RotateLokoTail(tailTS, prevNode.Value);

                    break;

                }
                prevNode = prevNode.Previous;
            }



            float forwardedWayPoint = lokoTS.InverseTransformPoint(wayNode.Value.position).z;

            if (Mathf.Abs(forwardedWayPoint) <= 0.05f || forwardedWayPoint < 0)
            { //  wayList.Remove(wayList[0]);
                wayNode = wayNode.Next;

                if (++currWayCount >= wayNextCount)
                {
                    wayNextCount = BlockNextRail(++railToBlock);
                    currWayCount = 0;
                    //Debug.Log("Block " + railToBlock + " rail");
                }
            }

            if (wayNode.Next == null)
            {
                float forwardLokoPoint = lokoChild.InverseTransformPoint(wayNode.Value.position).z;
                if (forwardLokoPoint <= 0.1f)//Finish
                {
                    GameplayController.controller.TrainFinish(OnFinishTail);
                    wayNode = wayNode.Next;
                }
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 11)
        {
            Debug.Log("Contact: " + other.gameObject.name);
        }
    }

    public void PausePath()
    {
        changeSpeed(true);
        /*
        if (wayCorout != null)
        {
            StopCoroutine(wayCorout);
            wayCorout = null;
        }
        */
    }
    public void ContinuePath()
    {
        changeSpeed(false);
        //Debug.Log("Train continue path");
        //wayCorout = StartCoroutine(WayCoroutine());
    }
}
