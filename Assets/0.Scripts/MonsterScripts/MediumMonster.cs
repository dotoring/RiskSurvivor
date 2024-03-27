using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumMonster : Monster
{
    [Header("for this monster")]
    public GameObject bulletPref;
    public GameObject chargeEffect;
    public Transform monsterShotPoint;
    public float bulletSpeed;
    Vector3 target;

    public override void Init()
    {
        base.Init();
        attackTimeoutDelta = 0.5f; //공격 재사용 대기 시간 초기화
        shootTimeoutDelta = 1.0f; //원거리 공격 재사용 대기 시간 초기화
        waitTimeoutDelta = 0.0f;
    }

    public override void Respawn()
    {
        base.Respawn();
        attackTimeoutDelta = 0.5f; //공격 재사용 대기 시간 초기화
        shootTimeoutDelta = 1.0f; //원거리 공격 재사용 대기 시간 초기화
        waitTimeoutDelta = 0.0f;
    }

    public override void CheckState(Transform playerTr)
    {
        Vector3 dis = playerTr.position - transform.position;
        float distance = Vector3.SqrMagnitude(dis); //플레이어와 몬스터 사이의 거리 제곱

        if (monStat == MonsterStat.Death)
        {
            return;
        }
        else if (waitTimeoutDelta > 0.0f || attackTimeoutDelta > 0.0f) //대기 시간 이면
        {
            monStat = MonsterStat.Idle;
        }
        else if (distance < attackRange * attackRange) //플레이어가 공격 사거리 안에 들어오면 공격
        {
            monStat = MonsterStat.MeleeAttack;
        }
        else if (distance < shootRange * shootRange && shootTimeoutDelta <= 0.0f && attackTimeoutDelta <= 0.0f) //플레이어가 원거리 공격 사거리 안에 들어오면 원거리 공격
        {
            monStat = MonsterStat.RangeAttack;
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
            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", false); //대기 애니메이션 정지

            attackTimeoutDelta = attackTimeout; //공격 대기시간 초기화
            animator.SetTrigger("OnAttack"); //공격 애니메이션 재생
        }

        if (attackTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            attackTimeoutDelta -= Time.deltaTime;
        }
    }

    public override void Shoot(Transform playerTr, Animator animator)
    {
        target = playerTr.position; //목표점 지정
        target.y += 1.0f; //플레이어 목표점 키에 맞춰 수정

        if (monStat == MonsterStat.RangeAttack && shootTimeoutDelta <= 0.0f)
        {
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
    }

    public void Shooting() //사격 함수(애니메이션 이벤트)
    {
        chargeEffect.SetActive(false);
        Vector3 shotDir = (target - monsterShotPoint.position).normalized; //발사 목표지점 설정
        //총알 생성
        GameObject bullet = Instantiate(bulletPref, monsterShotPoint.position, Quaternion.LookRotation(Vector3.forward));
        bullet.GetComponent<MonsterBulletCtrl>().damage = attackPower; //데미지 설정
        bullet.GetComponent<Rigidbody>().AddForce(shotDir * bulletSpeed); //총알 발사
    }

    public override void Move(Transform playerTr, Animator animator)
    {

        if (monStat == MonsterStat.Move)
        {
            animator.SetBool("IsWait", false); //대기 애니메이션 정지
            animator.SetBool("IsMove", true); //움직이는 애니메이션 재생

            Vector3 moveDir = playerTr.position - this.transform.position;
            moveDir.y = 0.0f;
            Vector3 moveVec = moveDir.normalized;
            transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World); //플레이어가 있는 방향으로 움직이기(월드 좌표계사용)
        }
    }

    public void Wait(Animator animator)
    {
        if(monStat == MonsterStat.Idle)
        {
            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", true); //대기 애니메이션 재생

            waitTimeoutDelta -= Time.deltaTime; //대기 시간 감소
        }
    }

    public override void Death(Animator animator, GameMgr gameMgr)
    {
        base.Death(animator, gameMgr);
    }

    public override void Action(Transform playerTr, Animator animator, GameMgr gameMgr)
    {
        if(monStat != MonsterStat.Death)
        {
            transform.LookAt(playerTr); //플레이어가 있는 방향 항상 바라보기
        }

        CheckState(playerTr);
        Move(playerTr, animator);
        Wait(animator);
        Shoot(playerTr, animator);
        Attack(animator);
        Death(animator, gameMgr);
    }
}
