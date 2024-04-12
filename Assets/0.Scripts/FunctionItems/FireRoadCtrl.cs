using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoadCtrl : FunctionItemClass
{
    public float damageRate; //데미지 비율
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
            //데미지 설정 (플레이어 공격력 * 배율 * 갯수)
            float dmg = PlayerValue.Instance.attackDamage * damageRate * quantity;
            if (other.GetComponent<MonsterCtrl>() != null) //부위 단일 개체일 경우
            {
                //지속피해 주기
                other.GetComponent<MonsterCtrl>().Damaged(dmg * Time.deltaTime);
            }
            if (other.GetComponent<MonsterColliderParts>() != null) //부위별로 충돌체가 있는 경우
            {
                //해당 몬스터의 부위를 통해 몬스터컨트롤 가져오기
                MonsterCtrl mc = other.GetComponent<MonsterColliderParts>().monsterCtrl;
                if (!mc.damageApplied)
                {
                    //지속피해 주기
                    mc.GetComponent<MonsterCtrl>().Damaged(dmg * Time.deltaTime);
                }
            }
        }
    }
}
