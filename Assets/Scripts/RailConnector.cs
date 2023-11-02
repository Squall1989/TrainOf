using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailConnector : MonoBehaviour
{
    public RailConnector opposingConnector;
    private RailConnector connectedRail;
    public delegate void ConnectorNew(RailConnector connector);
    public ConnectorNew connectorNew, disconnectorOld;
    private StartTail watcher;
    public Transform[] wayPoints;

    private void OnEnable()
    {
        if(wayPoints.Length == 0)
        {
            Transform[] tempPoints = opposingConnector.GetWayPoints();
            wayPoints = new Transform[tempPoints.Length];
            if (tempPoints.Length == 0)
                return;
            for (int i = 0; i < tempPoints.Length; i++)
            {
                int endI = tempPoints.Length - 1 - i;
                wayPoints[i] = tempPoints[endI];
            }
        }

    }
    private void Start()
    {
        //watcher = null;
        for (int i = 0; i < 30; i++)
        {
            if(i != gameObject.layer)
                Physics.IgnoreLayerCollision(gameObject.layer, i);
        }
    }

    public Tail GetParentTale()
    {
        Tail parent = transform.parent.parent.GetComponent<Tail>();
        return parent;
    }

    public Transform[] GetWayPoints()
    {
        return wayPoints;
    }

    public RailConnector GetOppositeConnector()
    {
        return opposingConnector;
    }

    public RailConnector GetNextConnect()
    {
        
        return connectedRail;
    }



    public void OnTriggerEnter(Collider other)
    {
        
        connectedRail = other.GetComponents<RailConnector>()[0];


        if (connectorNew != null)
        {
            Debug.Log("Invoke Watcher");
            connectorNew.Invoke(this);
            connectorNew = null;
        }
        //EventManager.Invoke("connect", new BecomeEvent(true, 0, 0));
    }
    private void OnTriggerExit(Collider other)
    {
        if (disconnectorOld != null)
        {
            GetOppositeConnector().connectorNew = null;
            Debug.Log("Invoke Disconnector");
            disconnectorOld.Invoke(other.GetComponent<RailConnector>());
            disconnectorOld = null;
        }
        //if (connectedRail == other.GetComponent<RailConnector>())
        connectedRail = null;

        
        //EventManager.Invoke("connect", new BecomeEvent(true, 0, 0));
    }
}

