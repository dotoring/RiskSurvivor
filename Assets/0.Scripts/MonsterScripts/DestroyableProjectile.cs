using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableProjectile : Monster
{
    public GameObject impactEffect;

    public override void Action(Transform playerTr, Animator animator, GameMgr gameMgr)
    {
        Death(animator, gameMgr);
    }

    public override void Attack(Animator animator)
    {
        throw new System.NotImplementedException();
    }

    public override void CheckState(Transform playerTr)
    {
        throw new System.NotImplementedException();
    }

    public override void Move(Transform playerTr, Animator animator)
    {
        throw new System.NotImplementedException();
    }

    public override void Shoot(Transform playerTr, Animator animator)
    {
        throw new System.NotImplementedException();
    }

    public override void Death(Animator animator, GameMgr gameMgr)
    {
        if (monCurHP <= 0.0f && monStat != MonsterStat.Death)
        {
            monStat = MonsterStat.Death; //사망 상태로 변경
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Untagged") //충돌체가 플레이어의 발사체가 아닐경우
        {
            GameObject go = Instantiate(impactEffect, transform.position, Quaternion.identity);
            go.GetComponent<CheckPlayerInArea>().damage = attackPower*4.0f;
            Destroy(go, 2.0f);
            Destroy(gameObject);
        }
    }
}
