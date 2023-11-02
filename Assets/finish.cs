using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finish : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent.GetComponent<Train>().ComeOnFinish();
    }
}
