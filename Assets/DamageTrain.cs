using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrain : MonoBehaviour
{
    private const float damageTime = 1f;
    private Collider collider_;
    // Start is called before the first frame update
    void Start()
    {
        collider_ = GetComponent<BoxCollider>();
        collider_.enabled = false;
    }

    private void OnEnable()
    {
        EventManager.AddListener("TrainStart", TrainStarted);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener("TrainStart", TrainStarted);

    }

    private void TrainStarted(BecomeEvent BE)
    {
        collider_.enabled = BE.come;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "train")
            return;
        Debug.Log("Damage trigger");
        Train train_ = other.transform.parent.GetComponent<Train>();
        if (!train_)
            return;
        Mechanics.mechanics.HealMinus();
        //StartCoroutine(damageCoroutine(train_));
    }

    private IEnumerator damageCoroutine(Train train)
    {
        train.PausePath();
        yield return new WaitForSeconds(damageTime);
        train.ContinuePath();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
