using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "MapSensor")
        {
            Debug.Log("in");
            MapCtrl.flag = true;
        }
    }
}
