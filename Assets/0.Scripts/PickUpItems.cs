using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Micosmo.SensorToolkit;

public class PickUpItems : MonoBehaviour
{
    public Sensor pickUpSensor;

    void Update()
    {
        foreach (GameObject item in pickUpSensor.Detections)
        {
            Vector3 dir = transform.position - item.transform.position;
            float dis = dir.magnitude;
            if (dis >= 0.2f)
            {
                item.transform.Translate(dir.normalized * Time.deltaTime * 20.0f);
                //item.transform.position = Vector3.Lerp(item.transform.position, transform.position, Time.deltaTime * 10.0f);
            }
            else
            {
                Destroy(item);
            }
        }
    }
}
