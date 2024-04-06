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
    public float monBasicMaxHP; //몬스터 기본 최대 체력
    public float monMaxHP; //몬스터 최대 체력
    [HideInInspector] public float monCurHP; //몬스터 체력
    public float moveSpeed; //몬스터 이동속도
    public GameObject expGemPref; //몬스터 드랍 경험치

    //몬스터 공격 관련 변수
    public float basicAttackPower; //몬스터 기본 공격력
    public float attackPower; //몬스터 공격력
    public float attackTimeout; //몬스터 공격 주기
    [HideInInspector] public float attackTimeoutDelta;
    public float attackRange; //몬스터 공격 사거리

    public float shootTimeout; //몬스터 원거리 공격 주기
    [HideInInspector] public float shootTimeoutDelta;
    public float shootRange; //몬스터 원거리 공격 사거리

    public float waitTimeout; //공격 후 대기시간
    [HideInInspector] public float waitTimeoutDelta;

    public MonsterStat monStat;

    [Header("Player Item Effect")]
    public GameObject explosionEffect;


    public virtual void Init()
    {
        GameMgr gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        monMaxHP = monBasicMaxHP + ((monBasicMaxHP * 0.3f) * (int)(gameMgr.playTime / 60)); //시간별 몬스터 최대 체력 조절
        monCurHP = monMaxHP;
        attackPower = basicAttackPower + ((basicAttackPower * 0.2f) * (gameMgr.playTime / 60));
        monStat = MonsterStat.Spawn;
    }

    public abstract void CheckState(Transform playerTr);
    public abstract void Move(Transform playerTr, Animator animator);
    public abstract void Attack(Animator animator);
    public abstract void Shoot(Transform playerTr, Animator animator);
    public virtual void Death(Animator animator, GameMgr gameMgr)
    {
        if (monCurHP <= 0.0f && monStat != MonsterStat.Death)
        {
            animator.SetTrigger("OnDeath"); //사망 애니메이션 재생
            monStat = MonsterStat.Death; //사망 상태로 변경
            gameObject.GetComponent<Rigidbody>().useGravity = true; //중력 활성화
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            //도깨비불 아이템 보유시
            if(PlayerValue.Instance.ringOfDoom >= 1)
            {
                if(explosionEffect != null)
                {
                    Instantiate(explosionEffect, transform.position, Quaternion.identity);
                }
            }

            //경험치 보석 생성
            Instantiate(expGemPref, transform.position, Quaternion.identity);
            gameMgr.monstersTr.Remove(transform); //몬스터들 위치 리스트에서 제거

            StartCoroutine(Disable());
        }
    }
    public virtual void Respawn()
    {
        GameMgr gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        monMaxHP = monBasicMaxHP + ((monBasicMaxHP * 0.3f) * (int)(gameMgr.playTime / 60)); //시간별 몬스터 최대 체력 조절
        monCurHP = monMaxHP;
        attackPower = attackPower + ((attackPower * 0.2f) * (gameMgr.playTime / 180));
        monStat = MonsterStat.Spawn;

        gameObject.layer = LayerMask.NameToLayer("Monster");
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);
        yield return null;
    }

    public abstract void Action(Transform playerTr, Animator animator, GameMgr gameMgr);
}
