using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum MonsterStat
{
    Spawn,
    Move,
    Attack,
    Death
}

public class MonsterCtrl : MonoBehaviour
{
    Animator animator;
    GameObject player;
    Transform playerTr;
    PlayerValue playerVal;
    MonsterStat monStat;

    [Header("Monster Status")]
    public float monHP = 100.0f;
    public float moveSpeed = 1.0f;
    public int exp = 10; //몬스터가 주는 경험치량

    //몬스터 공격 관련 변수
    public float attackPower = 10.0f;
    public float attackTimeout = 0.5f;
    private float attackTimeoutDelta;
    public float attackRange = 1.5f;


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        playerTr = player.GetComponent<Transform>();
        playerVal = player.GetComponent<PlayerValue>();

        monStat = MonsterStat.Spawn;

        //공격 재사용 대기 시간 초기화
        attackTimeoutDelta = attackTimeout;
    }

    void Update()
    {
        CheckState();
        Move();
        Attack();
        Death();
    }

    //몬스터의 상태 확인 및 변경 함수
    void CheckState()
    {
        float distance = Vector3.Distance(playerTr.position, transform.position); //플레이어와 몬스터 사이의 거리

        if(monStat == MonsterStat.Death)
        {
            return;
        }
        else if (distance < attackRange) //플레이어가 공격 사거리 안에 들어오면 공격
        {
            monStat = MonsterStat.Attack;
        }
        else
        {
            monStat = MonsterStat.Move;
        }
    }

    //몬스터의 이동 함수
    void Move()
    {
        if(monStat == MonsterStat.Move)
        {
            animator.SetBool("IsMove", true); //움직이는 애니메이션 재생
            transform.LookAt(playerTr); //플레이어가 있는 방향 바라보기

            Vector3 moveDir = playerTr.position - this.transform.position;
            moveDir.y = 0.0f;
            Vector3 moveVec = moveDir.normalized;
            transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World); //플레이어가 있는 방향으로 움직이기(월드 좌표계사용)
        }
    }

    //몬스터의 공격 함수
    void Attack()
    {
        if(monStat == MonsterStat.Attack && attackTimeoutDelta <= 0.0f) //공격 대기시간이 끝나면 공격
        {
            Debug.Log("공격");
            attackTimeoutDelta = attackTimeout; //공격 대기시간 초기화
            animator.SetTrigger("OnAttack"); //공격 애니메이션 재생
        }

        if (attackTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            attackTimeoutDelta -= Time.deltaTime;
        }
    }

    //몬스터 사망 함수
    void Death()
    {
        if(monHP <= 0.0f && monStat != MonsterStat.Death)
        {
            animator.SetTrigger("OnDeath"); //사망 애니메이션 재생
            monStat = MonsterStat.Death; //사망 상태로 변경
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<CapsuleCollider>().enabled = false; //콜리더 비활성화

            //플레이어에게 몬스터 처치 경험치 부여
            playerVal.GainExp(exp);
        }
    }

    //몬스터가 피해를 받는 함수
    public void Damaged(int val)
    {
        monHP -= val;
    }
}
