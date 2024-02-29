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
    GameMgr gameMgr;
    MonsterStat monStat;

    [Header("Monster Status")]
    public float monMaxHP = 100.0f; //몬스터 최대 체력
    public float monCurHP; //몬스터 체력
    public float moveSpeed = 1.0f; //몬스터 이동속도
    public int exp = 10; //몬스터가 주는 경험치량

    //몬스터 공격 관련 변수
    public int attackPower = 10; //몬스터 공격력
    public float attackTimeout = 0.5f; //몬스터 공격 주기
    private float attackTimeoutDelta;
    public float attackRange = 1.0f; //몬스터 공격 사거리

    public Collider attackHitBox;


    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        playerTr = player.GetComponent<Transform>();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();

        monCurHP = monMaxHP;
        monStat = MonsterStat.Spawn;

        //공격 재사용 대기 시간 초기화
        attackTimeoutDelta = attackTimeout;
    }

    private void OnEnable()
    {
        monCurHP = monMaxHP;
        monStat = MonsterStat.Spawn;
        attackTimeoutDelta = attackTimeout;

        gameObject.GetComponent<Rigidbody>().useGravity = true; //중력 활성화
        gameObject.GetComponent<Rigidbody>().isKinematic = false; //외부 물리력 활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = true; //콜리더 활성화
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
        Vector3 dis = playerTr.position - transform.position;
        float distance = Vector3.SqrMagnitude(dis); //플레이어와 몬스터 사이의 거리 제곱

        if (monStat == MonsterStat.Death)
        {
            return;
        }
        else if (distance < attackRange * attackRange) //플레이어가 공격 사거리 안에 들어오면 공격
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
        if(monCurHP <= 0.0f && monStat != MonsterStat.Death)
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

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);
        yield return null;
    }

    //몬스터가 피해를 받는 함수
    public void Damaged(int val)
    {
        monCurHP -= val;
    }

    public void ActivateAttackHitbox() //공격 모션 시작시 호출될 함수
    {
        attackHitBox.enabled = true; //공격 판정 범위 활성화
    }

    public void DeactivateAttackHitbox() //공격 모션이 끝날 때 호출될 함수
    {
        attackHitBox.enabled = false; //공격 판정 범위 비활성화
    }

    private void OnTriggerEnter(Collider other) //공격 판정 범위에 콜리더가 들어왔을 때
    {
        if(other.tag == "Player") //플레이어면 데미지 주기
        {
            PlayerValue.Instance.PlayerTakeDamage(attackPower);
        }
    }
}
