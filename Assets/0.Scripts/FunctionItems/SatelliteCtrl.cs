using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

public class SatelliteCtrl : FunctionItemClass
{
    Transform playerTr;

    public float damageRate; //데미지
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
            if (other.GetComponent<MonsterCtrl>() != null) //부위 단일 개체일 경우
            {
                hitSound.Play();
                other.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.attackDamage * damageRate);
            }
            if (other.GetComponent<MonsterColliderParts>() != null) //부위별로 충돌체가 있는 경우
            {
                //해당 몬스터의 부위를 통해 몬스터컨트롤 가져오기
                MonsterCtrl mc = other.GetComponent<MonsterColliderParts>().monsterCtrl;
                if (!mc.damageApplied)
                {
                    hitSound.Play();
                    mc.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.attackDamage * damageRate);
                }
            }
        }
    }
}
