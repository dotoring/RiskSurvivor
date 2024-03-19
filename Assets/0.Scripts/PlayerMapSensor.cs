using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MapSensor")
        {
            MapCtrl.flag = true;
        }
    }
}
