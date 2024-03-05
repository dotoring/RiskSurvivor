using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterStat
{
    Spawn,
    Idle,
    Move,
    MeleeAttack,
    RangeAttack,
    SkillAttack,
    Death
}

public abstract class Monster : MonoBehaviour
{
    public float monMaxHP; //몬스터 최대 체력
    [HideInInspector] public float monCurHP; //몬스터 체력
    public float moveSpeed; //몬스터 이동속도
    public int exp; //몬스터가 주는 경험치량

    //몬스터 공격 관련 변수
    public int attackPower; //몬스터 공격력
    public float attackTimeout; //몬스터 공격 주기
    [HideInInspector] public float attackTimeoutDelta;
    public float attackRange; //몬스터 공격 사거리

    public float shootTimeout; //몬스터 원거리 공격 주기
    [HideInInspector] public float shootTimeoutDelta;
    public float shootRange; //몬스터 원거리 공격 사거리

    public float waitTimeout; //공격 후 대기시간
    [HideInInspector] public float waitTimeoutDelta;

    public MonsterStat monStat;


    public abstract void Init();

    public abstract void CheckState(Transform playerTr);
    public abstract void Move(Transform playerTr, Animator animator);
    public abstract void Attack(Animator animator);
    public virtual void Death(Animator animator, GameMgr gameMgr)
    {
        if (monCurHP <= 0.0f && monStat != MonsterStat.Death)
        {
            animator.SetTrigger("OnDeath"); //사망 애니메이션 재생
            monStat = MonsterStat.Death; //사망 상태로 변경
            gameObject.GetComponent<Rigidbody>().useGravity = false; //중력 비활성화
            gameObject.GetComponent<Rigidbody>().isKinematic = true; //외부 물리력 비활성화
            gameObject.GetComponent<CapsuleCollider>().enabled = false; //콜리더 비활성화

            //플레이어에게 몬스터 처치 경험치 부여
            PlayerValue.Instance.GainExp(exp);
            gameMgr.monstersTr.Remove(transform); //몬스터들 위치 리스트에서 제거

            StartCoroutine(Disable());
        }
    }
    public abstract void Respawn();

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);
        yield return null;
    }

    public abstract void Action(Transform playerTr, Animator animator, GameMgr gameMgr);
}
