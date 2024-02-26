using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SatelliteCtrl : MonoBehaviour
{
    ItemFunction itemFunction;
    public Transform playerTr;
    public float spinSpeed;

    void Start()
    {
        itemFunction = GameObject.Find("Player").GetComponent<ItemFunction>();
        playerTr = GameObject.Find("PlayerTransform").transform;
    }

    void Update()
    {
        Vector3 newPos = transform.position = playerTr.position + (transform.position - playerTr.position).normalized * itemFunction.satelliteRadius;
        newPos.y = transform.position.y;
        transform.RotateAround(playerTr.position, Vector3.up, spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            other.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.DamageCalc());
        }
    }
}
