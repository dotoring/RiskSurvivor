using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoadCtrl : FunctionItemClass
{
    public float damage; //데미지
    public int quantity;
    public float lifeTime; //지속시간
    float lifeTimeDelta;

    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    private void OnEnable()
    {
        lifeTimeDelta = lifeTime;
    }

    void Update()
    {
        if(lifeTimeDelta <= 0) //지속시간이 끝나면 제거
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        else
        {
            lifeTimeDelta -= Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            //시간에 따른 데미지 증가
            float dmg = damage + ((damage * 0.2f) * (int)(gameMgr.playTime / 60));
            //갯수에 따른 데미지 증가
            dmg *= 1.0f + (0.3f * quantity);
            //지속피해 주기
            other.GetComponent<MonsterCtrl>().Damaged(dmg * Time.deltaTime);
        }
    }
}
