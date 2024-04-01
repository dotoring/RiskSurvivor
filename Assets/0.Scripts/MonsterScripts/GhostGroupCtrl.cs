using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGroupCtrl : MonoBehaviour
{
    Vector3 moveDir;
    public float moveSpeed;
    public float lifeTime; //맵 밖으로 나갈 정도의 시간으로 설정

    void Start()
    {
        //스폰 된 순간의 플레이어가 있는 곳으로 이동 방향 정하기
        Transform player = GameObject.Find("PlayerTransform").GetComponent<Transform>();
        moveDir = player.position - this.transform.position;
        moveDir.y = 0.0f;

        //이동방향으로 회전시켜주기
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        gameObject.transform.rotation = targetRot;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.World); //플레이어가 있는 방향으로 움직이기(월드 좌표계사용)

        //일정 시간 후에 삭제
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0 )
        {
            Destroy(gameObject);
        }
    }
}
