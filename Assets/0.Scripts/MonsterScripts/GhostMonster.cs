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
}
