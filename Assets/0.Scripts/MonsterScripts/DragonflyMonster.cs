using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonflyMonster : Monster
{
    [Header("for this monster")]
    public GameObject bulletPref;
    public Transform monsterShotPoint;
    public float bulletSpeed;
    Vector3 target;
    float distanceFromPlayer;

    public override void Init()
    {
        monCurHP = monMaxHP; //몬스터 체력
        shootTimeoutDelta = 1.0f; //원거리 공격 재사용 대기 시간 초기화

        monStat = MonsterStat.Spawn;
    }

    public override void Respawn()
    {
        monCurHP = monMaxHP;
        monStat = MonsterStat.Spawn;
        shootTimeoutDelta = 1.0f; //원거리 공격 재사용 대기 시간 초기화


        gameObject.GetComponent<Rigidbody>().useGravity = true; //중력 활성화
        gameObject.GetComponent<Rigidbody>().isKinematic = false; //외부 물리력 활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = true; //콜리더 활성화
    }

    public override void CheckState(Transform playerTr)
    {
        Vector3 dis = playerTr.position - transform.position;
        distanceFromPlayer = Vector3.SqrMagnitude(dis); //플레이어와 몬스터 사이의 거리 제곱

        if (monStat == MonsterStat.Death)
        {
            return;
        }
        else if (distanceFromPlayer < shootRange * shootRange && shootTimeoutDelta <= 0.0f) //플레이어가 원거리 공격 사거리 안에 들어오면 원거리 공격
        {
            monStat = MonsterStat.RangeAttack;
        }
        else if(distanceFromPlayer < 5.0f*5.0f)
        {
            monStat = MonsterStat.Idle;
        }
        else
        {
            monStat = MonsterStat.Move;
        }
    }

    public override void Move(Transform playerTr, Animator animator)
    {
        transform.LookAt(playerTr); //플레이어가 있는 방향 바라보기

        if (monStat == MonsterStat.Move) //공격하는 동안에 이동x
        {
            animator.SetBool("IsWait", false); //대기 애니메이션 정지
            animator.SetBool("IsMove", true); //움직이는 애니메이션 재생

            Vector3 moveDir = playerTr.position - this.transform.position;
            Vector3 moveVec = moveDir.normalized;
            transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World); //플레이어가 있는 방향으로 움직이기(월드 좌표계사용)
            transform.Translate(Vector3.up * 0.7f * Time.deltaTime, Space.World); //y축을 계속 올려줘서 날아다니는 효과
        }
    }

    public override void Shoot(Transform playerTr, Animator animator)
    {
        if (monStat == MonsterStat.RangeAttack && shootTimeoutDelta <= 0.0f) //공격 대기시간이 끝나면 공격
        {
            shootTimeoutDelta = shootTimeout; //공격 대기시간 초기화
            animator.SetTrigger("OnAttack"); //공격 애니메이션 재생
            target = playerTr.position; //목표점 지정
            target.y += 1.0f; //플레이어 목표점 키에 맞춰 수정
        }

        if (shootTimeoutDelta > 0.0f) //공격 대기시간이 남아있으면 시간 감소시키기
        {
            shootTimeoutDelta -= Time.deltaTime;
        }
    }

    public void Shooting() //사격 함수(애니메이션 이벤트)
    {
        Vector3 shotDir = (target - monsterShotPoint.position).normalized; //발사 목표지점 설정
        float modifyDir = Mathf.Sqrt(Mathf.Sqrt(distanceFromPlayer)) / 5; //곡사를 위한 방향조정
        shotDir.y += modifyDir;
        //총알 생성
        GameObject bullet = Instantiate(bulletPref, monsterShotPoint.position, Quaternion.LookRotation(Vector3.forward));
        bullet.GetComponent<MonsterBulletCtrl>().damage = attackPower; //데미지 설정
        bullet.GetComponent<Rigidbody>().AddForce(shotDir * bulletSpeed); //총알 발사
    }

    public void Wait(Animator animator)
    {
        if (monStat == MonsterStat.Idle)
        {
            animator.SetBool("IsMove", false); //움직임 정지
            animator.SetBool("IsWait", true); //대기 애니메이션 재생
        }
    }

    public override void Action(Transform playerTr, Animator animator, GameMgr gameMgr)
    {
        CheckState(playerTr);
        Move(playerTr, animator);
        Shoot(playerTr, animator);
        Death(animator, gameMgr);
        Wait(animator);
    }

    public override void Attack(Animator animator)
    {
        throw new System.NotImplementedException();
    }
}
