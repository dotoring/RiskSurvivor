using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SatelliteCtrl : MonoBehaviour
{
    Transform playerTr;

    public int damage; //데미지
    public float spinSpeed; //회전 속도

    void Start()
    {
        playerTr = GameObject.Find("PlayerTransform").transform;
    }

    void Update()
    {
        //플레이어의 위치를 중심으로 y축으로만 회전
        transform.RotateAround(playerTr.position, Vector3.up, spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            other.GetComponent<MonsterCtrl>().Damaged(damage);
        }
    }
}
