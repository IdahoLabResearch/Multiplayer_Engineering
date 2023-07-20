using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPositionToPlayer : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
    }
}