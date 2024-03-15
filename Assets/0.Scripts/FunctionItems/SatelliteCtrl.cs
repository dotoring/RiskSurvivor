using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SatelliteCtrl : FunctionItemClass
{
    Transform playerTr;

    public float damage; //데미지
    public float spinSpeed; //회전 속도
    public AudioSource hitSound; //타격 사운드

    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
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
            hitSound.Play();
            float dmg = damage + ((damage * 0.2f) * (int)(gameMgr.playTime / 60));
            other.GetComponent<MonsterCtrl>().Damaged(dmg);
        }
    }
}
