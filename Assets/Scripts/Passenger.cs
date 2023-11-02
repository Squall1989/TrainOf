using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public GameObject canvasSpec;
    public float passengerSpeed;
    private Train train;
    private const float distForJumpTrain = .3f;

    // Start is called before the first frame update
    void Start()
    {
        float trainSlow = PlayerPrefs.GetFloat("lowSpeed");
        passengerSpeed = trainSlow > 0 ? trainSlow / 15f : 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "train")
            return;

        Train train_ = other.transform.parent.GetComponent<Train>();
        if (!train_)
            return;
        train = train_;
        train.PausePath();
        canvasSpec.SetActive(false);
        StartCoroutine(runToTrainCoroutine());
    }

    private IEnumerator runToTrainCoroutine()
    {
        Transform lokoPoint = train.LokoForPassenger();
        Transform tailPoint = train.TailForPassenger();
        float distToLoko = (lokoPoint.position - transform.position).magnitude;
        float distToTail = (tailPoint.position - transform.position).magnitude;
        transform.LookAt(lokoPoint);
        while(distToLoko > distForJumpTrain && distToTail > distForJumpTrain)
        {
            Vector3 vectToLoko = (lokoPoint.position - transform.position);
            Vector3 vectToTail = (tailPoint.position - transform.position);
            if (vectToLoko.magnitude < vectToTail.magnitude)
            {

                transform.position += vectToLoko.normalized * passengerSpeed * Time.deltaTime;
            }
            else
            {

                transform.position += vectToTail.normalized * passengerSpeed * Time.deltaTime;
            }
            distToLoko = (lokoPoint.position - transform.position).magnitude;
            distToTail = (tailPoint.position - transform.position).magnitude;
            yield return null;
        }
        train.ContinuePath();
        GameplayController.controller.PassengerPlus();
        gameObject.SetActive(false);
    }
}
