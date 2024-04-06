using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMonster : Monster
{
    public override void Action(Transform playerTr, Animator animator, GameMgr gameMgr)
    {
        Death(animator, gameMgr);
    }

    public override void Death(Animator animator, GameMgr gameMgr)
    {
        if (monCurHP <= 0.0f && monStat != MonsterStat.Death)
        {
            //도깨비불 아이템 보유시
            if (PlayerValue.Instance.ringOfDoom >= 1)
            {
                if (explosionEffect != null)
                {
                    Instantiate(explosionEffect, transform.position, Quaternion.identity);
                }
            }

            //경험치 보석 생성
            Instantiate(expGemPref, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
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

    private void OnTriggerEnter(Collider other) //공격 판정 범위에 콜리더가 들어왔을 때
    {
        if (other.tag == "Player") //플레이어면 데미지 주기
        {
            PlayerValue.Instance.PlayerTakeDamage(attackPower);
        }
    }
}
