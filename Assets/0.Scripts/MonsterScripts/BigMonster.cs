using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMonster : Monster
{
    [Header("for this monster")]
    public GameObject bulletPref;
    public GameObject missilePref;
    public GameObject spikePref;
    public GameObject chargeEffect;
    public AudioSource chargeSound;
    public AudioSource shootSound;
    public AudioSource rageSound;
    public Transform monsterBulletShotPoint;
    public Transform monsterMissileShotPoint;

    public float rageTimeout; //몬스터 분노 공격 주기
    [HideInInspector] public float rageTimeoutDelta;
    public float rotSpeed;

    public bool isLookAtPlayer = false;
    Vector3 target;

    public override void Init()
    {
        base.Init();
        shootTimeoutDelta = 3.0f;
        rageTimeoutDelta = 5.0f;
        attackTimeoutDelta = 2.0f;
        waitTimeoutDelta = 0.0f;
    }

    public override void Action(Transform playerTr, Animator animator, GameMgr gameMgr)
    {
        target = playerTr.position; //목표점 지정
        target.y += 1.0f; //플레이어 목표점 키에 맞춰 수정

        if (monStat != MonsterStat.Death && isLookAtPlayer == true) //죽은 상태 제외
        {
            //transform.LookAt(playerTr); //플레이어가 있는 방향 항상 바라보기

            Vector3 dir = target - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, rot.eulerAngles.y, 0f), rotSpeed * Time.deltaTime);
        }

        Attack(animator);
        Shoot(playerTr, animator);
        Rage(playerTr, animator);
        Wait(animator);
        CheckState(playerTr);
        Move(playerTr, animator);
        Death(animator, gameMgr);
    }


    public override void CheckState(Transform playerTr)
    {
        Vector3 dis = playerTr.position - transform.position;
        float distance = Vector3.SqrMagnitude(dis); //플레이어와 몬스터 사이의 거리 제곱

        if (monStat == MonsterStat.Death)
        {
            return;
        }
        else if (waitTimeoutDelta > 0.0f) //공격 대시시간
        {
            monStat = MonsterStat.Idle;
        }
        else if(monCurHP / monMaxHP < 0.5f && rageTimeoutDelta <= 0.0f) //체력이 50%미만일 때
        {
            monStat = MonsterStat.SkillAttack;
        }
        else if (monCurHP / monMaxHP < 0.9f && shootTimeoutDelta <= 0.0f) //체력이 90%미만일 때
        {
            monStat = MonsterStat.RangeAttack;
        }
        else if (attackTimeoutDelta <= 0.0f && distance < shootRange * shootRange) //플레이어가 사거리 안에 있을 때
        {
            monStat = MonsterStat.MeleeAttack;
        }
        else if (distance > attackRange * attackRange) //attackRange를 플레이어와 일정 거리 이상 다가가지 않도록 사용
        {
            monStat = MonsterStat.Move;
        }
        else //일정 거리 내로 들어오면 대기
        {
            monStat = MonsterStat.Idle;
        }
    }

    public override void Attack(Animator animator)
    {
        if (monStat == MonsterStat.MeleeAttack) //공격 대기시간이 끝나면 공격
        {
            isLookAtPlayer = true;
            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", false); //대기 애니메이션 정지

            attackTimeoutDelta = attackTimeout; //공격 대기시간 초기화
            animator.SetTrigger("OnAttack"); //공격 애니메이션 재생
            waitTimeoutDelta = waitTimeout;
        }

        if (attackTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            attackTimeoutDelta -= Time.deltaTime;
        }
    }

    public void Attacking() //사격 함수(애니메이션 이벤트)
    {
        //방사형으로 투사체 발사
        for (int i = -3; i < 4; i++)
        {
            // 각도 계산
            float angle = 90 * i / 7;
            // 각도에 따라 방향 벡터 계산
            Vector3 shotDir = (target - monsterBulletShotPoint.position).normalized; //발사 목표지점 설정
            shotDir = Quaternion.Euler(0, angle, 0) * shotDir;
            //총알 생성
            GameObject bullet = Instantiate(bulletPref, monsterBulletShotPoint.position, Quaternion.LookRotation(shotDir));
            bullet.GetComponent<MonsterBulletCtrl>().damage = attackPower; //데미지 설정
            bullet.GetComponent<Rigidbody>().AddForce(shotDir * 800); //총알 발사
        }
        isLookAtPlayer = false;
    }

    public override void Move(Transform playerTr, Animator animator)
    {
        if (monStat == MonsterStat.Move)
        {
            animator.SetBool("IsMove", true); //움직이는 애니메이션 재생
            animator.SetBool("IsWait", false); //대기 애니메이션 정지

            //Vector3 moveDir = playerTr.position - this.transform.position;
            //moveDir.y = 0.0f;
            //Vector3 moveVec = moveDir.normalized;
            //transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World); //플레이어가 있는 방향으로 움직이기(월드 좌표계사용)

            agent.isStopped = false;
            agent.SetDestination(playerTr.position); //네비게이션을 이용한 이동
        }
        else
        {
            agent.isStopped = true; //관성 움직임 제어
        }
    }

    public override void Shoot(Transform playerTr, Animator animator)
    {
        if (monStat == MonsterStat.RangeAttack)
        {
            isLookAtPlayer = true;

            shootTimeoutDelta = shootTimeout; //공격 대기시간 초기화

            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", false); //대기 애니메이션 정지
            animator.SetTrigger("OnShoot"); //공격 애니메이션 재생
            waitTimeoutDelta = waitTimeout; //대기 시간 시작
        }

        if (shootTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            shootTimeoutDelta -= Time.deltaTime;
        }
    }

    public void ChargingStart() //원거리 공격 충전 이펙트(애니메이션 이벤트)
    {
        chargeEffect.SetActive(true);
        chargeSound.Play();
    }

    public void Shooting() //사격 함수(애니메이션 이벤트)
    {
        chargeEffect.SetActive(false);
        shootSound.Play();
        Vector3 shotDir = (target - monsterMissileShotPoint.position).normalized; //발사 목표지점 설정
        //총알 생성
        GameObject bullet = Instantiate(missilePref, monsterMissileShotPoint.position, Quaternion.LookRotation(Vector3.forward));
        bullet.GetComponent<DestroyableProjectile>().basicAttackPower = basicAttackPower; //데미지 설정

        isLookAtPlayer = false;
    }

    public void Rage(Transform playerTr, Animator animator)
    {
        if (monStat == MonsterStat.SkillAttack)
        {
            rageTimeoutDelta = rageTimeout; //공격 대기시간 초기화
            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", false); //대기 애니메이션 정지
            animator.SetTrigger("OnRage"); //공격 애니메이션 재생
            rageSound.Play();
            waitTimeoutDelta = waitTimeout; //대기 시간 시작
        }

        if (rageTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            rageTimeoutDelta -= Time.deltaTime;
        }
    }

    public void RageAttack() //분노 공격 함수(애니메이션 이벤트)
    {
        StartCoroutine(RageAttackCoroutine());
    }

    IEnumerator RageAttackCoroutine()
    {
        for(int i = 0; i < 5; i++)
        {
            //스파이크 공격 생성
            GameObject spike = Instantiate(spikePref);
            spike.GetComponent<MonsterSpikeCtrl>().damage = attackPower * 2.0f;
            //위치 조정
            Vector3 playerPos = target;
            playerPos.y = 0.0f;
            spike.transform.position = playerPos;
            //반복하는 동안 0.5초마다 생성
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    public void Wait(Animator animator)
    {
        if (monStat == MonsterStat.Idle)
        {
            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", true); //대기 애니메이션 재생

            waitTimeoutDelta -= Time.deltaTime; //대기 시간 감소
        }
    }

    public override void Death(Animator animator, GameMgr gameMgr)
    {
        base.Death(animator, gameMgr);
        if(monStat == MonsterStat.Death)
        {
            gameMgr.KillAllEnemy();
        }
    }
}
