using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SmallMonster : Monster
{
    public override void Init()
    {
        monCurHP = monMaxHP; //몬스터 체력
        attackTimeoutDelta = attackTimeout; //공격 재사용 대기 시간 초기화

        monStat = MonsterStat.Spawn;
    }

public override void CheckState(Transform playerTr)
    {
        Vector3 dis = playerTr.position - transform.position;
        float distance = Vector3.SqrMagnitude(dis); //플레이어와 몬스터 사이의 거리 제곱

        if (monStat == MonsterStat.Death)
        {
            return;
        }
        else if (distance < attackRange * attackRange) //플레이어가 공격 사거리 안에 들어오면 공격
        {
            monStat = MonsterStat.MeleeAttack;
        }
        else
        {
            monStat = MonsterStat.Move;
        }
    }

    public override void Attack(Animator animator)
    {
        if (monStat == MonsterStat.MeleeAttack && attackTimeoutDelta <= 0.0f) //공격 대기시간이 끝나면 공격
        {
            attackTimeoutDelta = attackTimeout; //공격 대기시간 초기화
            animator.SetTrigger("OnAttack"); //공격 애니메이션 재생
        }

        if (attackTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            attackTimeoutDelta -= Time.deltaTime;
        }
    }

    public override void Move(Transform playerTr, Animator animator)
    {
        if (monStat == MonsterStat.Move)
        {
            animator.SetBool("IsMove", true); //움직이는 애니메이션 재생
            transform.LookAt(playerTr); //플레이어가 있는 방향 바라보기

            Vector3 moveDir = playerTr.position - this.transform.position;
            moveDir.y = 0.0f;
            Vector3 moveVec = moveDir.normalized;
            transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World); //플레이어가 있는 방향으로 움직이기(월드 좌표계사용)
        }
    }

    public override void Death(Animator animator, GameMgr gameMgr)
    {
        base.Death(animator, gameMgr);
    }

    public override void Respawn()
    {
        monCurHP = monMaxHP;
        monStat = MonsterStat.Spawn;
        attackTimeoutDelta = attackTimeout;

        gameObject.GetComponent<Rigidbody>().useGravity = true; //중력 활성화
        gameObject.GetComponent<Rigidbody>().isKinematic = false; //외부 물리력 활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = true; //콜리더 활성화
    }

    public override void Action(Transform playerTr, Animator animator, GameMgr gameMgr)
    {
        CheckState(playerTr);
        Move(playerTr, animator);
        Attack(animator);
        Death(animator, gameMgr);
    }
}
