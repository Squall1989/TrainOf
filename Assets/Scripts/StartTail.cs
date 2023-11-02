using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTail : Tail
{
    private bool trainStarted;
    private Train train;
    private const float checkTime = .1f;
    private List<checkWay> connectorList;
    private checkWay[] connectorTempList;
    private void Awake()
    {
        connectorList = new List<checkWay>();
        connectorTempList = new checkWay[0];
        StartCoroutine(connectCheck());
        //EventManager.AddListener("connect", ConnectNewTail);
        
    }
    public RailConnector startConnector1;


    private void OnEnable()
    {
        EventManager.AddListener("PlayLevel", LevelStart);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener("PlayLevel", LevelStart);

    }


    void Start()
    {
        if (GameplayController.controller)
        {
            GameplayController.controller.SetStartTail(this);
            if (!train && GameplayController.controller.IsLevelPlaying())
            {

                AddNewSpecial(ControlTales.controlTales.GetSpecial('t'), 't');
                train = specialGraphic.GetComponent<Train>();
            }
        }
    }

    private void LevelStart(BecomeEvent BE)
    {
        
    }

    public bool CheckAllConnections()
    {
        if (startConnector1.GetNextConnect())//Первый контакт старта
        {

            if (CheckConnections(startConnector1.GetNextConnect()))
            {
                return true;
            }
            //else
              //  return false;
        }
        if (startConnector1.GetOppositeConnector().GetNextConnect())//Второй контакт старта
        {

            if (CheckConnections(startConnector1.GetOppositeConnector().GetNextConnect()))
            {

                return true;
            }
            //else
              //  return false;
        }
        return false;

    }

    IEnumerator connectCheck()
    {
        while(!trainStarted)
        {
            yield return new WaitForSeconds(checkTime);
            CheckAllConnections();
        }
    }

    public void DisconnectTail(RailConnector lostConnector)
    {

        train.DisconnectConnector(lostConnector.GetOppositeConnector());
        
        CheckConnections(connectorTempList[0].connector);
        Debug.Log("(connectorTempList: " + connectorTempList.Length);
        DelegateConnect(connectorTempList[connectorTempList.Length-1].connector);
    }

    public void ConnectNewTail(RailConnector newConnector)
    {
        CheckConnections(newConnector.GetOppositeConnector());
        //Train train = specialGraphic.GetComponent<Train>();
        for (int i = 1; i < connectorTempList.Length; i++)
        {
            RailConnector railConnector = connectorTempList[i].connector;
            train.AddConnector(railConnector);
            DelegateConnect(railConnector);

            //railConnector.GetParentTale().BlockTail(true);
        }
        SetDisconnectDelegates();

    }

    public void StopTrain()
    {
        train.Stop();
        //specialGraphic.GetComponent<Train>().Stop();
    }

    private void DelegateConnect(RailConnector lastConnector)
    {
        //lastConnector = connectorTempList[connectorTempList.Length - 1].connector;
        lastConnector.GetOppositeConnector().connectorNew = new RailConnector.ConnectorNew(ConnectNewTail);

    }

    public bool StartTrainFromTimer()
    {
        StopAllCoroutines();
        if (!CheckAllConnections())
        {
            if (connectorTempList.Length == 0)
            {
                return false;
            }
            else
            {
                connectorList.Clear();
                connectorList.AddRange(connectorTempList);
                StartTrain();

                SetDisconnectDelegates();
                DelegateConnect(connectorList[connectorList.Count-1].connector);
            }
        }
        return true;
    }

    private void SetDisconnectDelegates()
    {
        for(int i = 0; i < connectorTempList.Length; i++)
        {
            connectorTempList[i].connector.disconnectorOld = new RailConnector.ConnectorNew(DisconnectTail);
        }
    }
    private bool CheckConnections(RailConnector startConnector_)
    {
        connectorList.Clear();
        connectorTempList = new checkWay[0];
        checkWay firstWay;
        firstWay.connector = startConnector_;
        firstWay.firstCheck = false;
        connectorList.Add(firstWay);


        checkWay lastWay = firstWay;
        while (connectorList.Count > 0 && !lastWay.firstCheck)
        {
            //connectorTempList = connectorList.ToArray();

            RailConnector nextCheck = CheckNext();
            if (nextCheck)
            {

                checkWay nextChecked;
                nextChecked.connector = nextCheck;
                nextChecked.firstCheck = false;
                connectorList.Add(nextChecked);

                if (connectorList.Count > connectorTempList.Length)//Для механики таймера
                {
                    connectorTempList = connectorList.ToArray();

                }

                Tail lastTail = nextCheck.GetParentTale();

                if (lastTail.taleType == TaleType.finish)
                {

                    if (train)
                    {
                        //Debug.Log("Finish"); 
                        TailsTable.talesTable.BlockAllTails();
                        StartTrain();
                    }
                    return true;
                }
            }
            else if(FindAltWay())//Возврат назад до первой развилки
            {
                continue;
            }
            if (connectorList.Count == 0)
                break;
            lastWay = connectorList[connectorList.Count - 1];
            if (connectorList.Count > connectorTempList.Length)//Для механики таймера
            {
                connectorTempList = connectorList.ToArray();
            }
        }

        return false;
    }


    private void StartTrain()
    {
        List<RailConnector> wayList = new List<RailConnector>();
        for(int i =0; i < connectorList.Count; i++)
        {
            RailConnector railConnector = connectorList[i].connector;
            wayList.Add(railConnector);
            //railConnector.GetParentTale().BlockTail(true);
        }
        train.StartForWayPoints(wayList);
        trainStarted = true;
        EventManager.Invoke("TrainStart", new BecomeEvent(true, 0,0));
    }

    private RailConnector CheckNext()
    {
        RailConnector opposingConnect = connectorList[connectorList.Count - 1].connector.GetOppositeConnector();
        if(!opposingConnect)
        {
            Debug.LogError("Not have opposing");
            return null;
        }
        RailConnector nextConnect = opposingConnect.GetNextConnect();

        return nextConnect;
    }

    private bool FindAltWay()
    {
        checkWay way = connectorList[connectorList.Count - 1];
        //int curr = connectorList.Count - 1;
        while (way.firstCheck == false && connectorList.Count > 0)
        {
            if(way.connector.GetComponents<RailConnector>().Length <= 1)
            {
                connectorList.Remove(way);
                if(connectorList.Count > 0)
                    way = connectorList[connectorList.Count - 1];
                continue;
            }
            RailConnector secondConnector = way.connector.GetComponents<RailConnector>()[1];
            if (secondConnector)
            {
                way.connector = secondConnector;
                way.firstCheck = true;
                connectorList[connectorList.Count - 1] = way;
                Debug.Log("Second Way");
                return true;
            }
            else
            {
                connectorList.Remove(way);
                way = connectorList[connectorList.Count - 1];
            }
        }
        return false;
    }

    
}



public struct checkWay
{
    public RailConnector connector;
    public bool firstCheck;
}
