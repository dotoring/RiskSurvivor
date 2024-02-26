using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetdoleeCtrl : MonoBehaviour
{
    GameMgr gameMgr;
    Transform closestMonster; //가장 가까운 몬스터 위치
    GameObject player;
    Quaternion targetRotation; //목표 방향
    Rigidbody rb;

    public int damage = 30; //epalwl
    public float trackingRange = 10f; //추적 범위
    public float moveSpeed = 1f; //이동 속도
    public float bounceForce; //충돌 후 튕겨나오는 힘

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어에서 일정 거리 이상 멀어지면 플레이어 근처로 재소환
        ResetPosition();

        closestMonster = GetClosestMonster();

        if(closestMonster != null) //탐지 범위 내에 가장 가까운 몬스터가 있다면
        {
            TrackingObejct(closestMonster);
        }
        else //없으면 플레이어에게 돌아오기
        {
            TrackingObejct(player.transform);
        }
    }

    void TrackingObejct(Transform target)
    {
        Vector3 moveDir = target.position - this.transform.position;
        moveDir.y = 0.0f;
        Vector3 moveVec = moveDir.normalized; //이동 방향 설정
        targetRotation = Quaternion.LookRotation(moveVec); //바라보는 방향 설정
        transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f); //목표가 있는 방향 바라보기
        transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.World); //목표가 있는 방향으로 움직이기(월드 좌표계사용)
    }

    Transform GetClosestMonster() //가장 가까이 있는 몬스터 탐색 함수
    {
        closestMonster = null;
        float closestDistanceSqr = Mathf.Infinity;

        // 모든 몬스터들의 위치를 확인하고 가장 가까운 몬스터 선택
        foreach (Transform monster in gameMgr.monstersTr)
        {
            Vector3 directionToTarget = monster.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            // 추적 범위 안에 있는 몬스터인지 확인, 가장 가까운 몬스터인지 확인
            if (dSqrToTarget < trackingRange * trackingRange && dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget; //가장 가까운 거리 재설정
                closestMonster = monster; //가장 가까운 몬스터 설정
            }
        }

        return closestMonster;
    }

    void ResetPosition() //플레이어에서 일정 거리 이상 멀어지면 플레이어 근처로 재소환하는 함수
    {
        Vector3 playerDistance = player.transform.position - transform.position;
        float sqrDistance = playerDistance.sqrMagnitude;
        if (sqrDistance > 30.0f * 30.0f)
        {
            float randX = Random.Range(0, 3.0f);
            float randZ = Random.Range(0, 3.0f);

            transform.position = player.transform.position + new Vector3(randX, 0.5f, randZ);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Monster")
        {
            // 충돌한 물체의 방향을 구함
            Vector3 collisionDirection = collision.contacts[0].point - transform.position;

            // 물체를 튕겨나오는 방향으로 힘을 가함
            rb.AddForce(-collisionDirection.normalized * bounceForce, ForceMode.Impulse);

            collision.gameObject.GetComponent<MonsterCtrl>().Damaged(50); //몬스터에게 피해주기
        }
    }
}
