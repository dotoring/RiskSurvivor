using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MissileCtrl : FunctionItemClass
{
    Transform closestMonster; //가장 가까운 몬스터 위치

    public float damageRate; //데미지
    public float trackingRange; //추적 범위
    public float moveSpeed; //이동 속도

    public float delayTime; // 미사일 발사 후 추적 딜레이시간

    public ParticleSystem explosionEffect;
    public AudioSource explosionSound;
    public GameObject model;

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        StartCoroutine("Tracking");
    }

    IEnumerator Tracking()
    {
        while(true)
        {
            if (delayTime > 0) //첫 딜레이 시간 동안 위로만 이동
            {
                delayTime -= Time.deltaTime;
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                //가장 가까운 몬스터 찾기
                closestMonster = GetClosestMonster(trackingRange);
                //가장 가까운 몬스터 추적하기
                if (closestMonster != null)
                {
                    TrackingObejct(closestMonster, moveSpeed);
                }
                else //추적할 몬스터가 없다면 움직이던대로 움직이기
                {
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    void TrackingObejct(Transform target, float moveSpeed)
    {
        Vector3 targetPosition = target.position;
        targetPosition.y += 1.0f;

        //목표물 방향으로 부드럽게 회전
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8.0f * Time.deltaTime);

        //바라보는 방향으로 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    IEnumerator Impact()
    {
        explosionEffect.Play(); //폭발 이펙트 재생
        explosionSound.Play(); //폭발 사운드 재생
        GetComponent<Collider>().enabled = false; //콜리더 비활성화
        model.SetActive(false); //모델링 비활성화
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject); //0.5초 후 삭제
        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopCoroutine("Tracking"); //추적 중지(안하면 폭발 이펙트도 따라감)
        StartCoroutine(Impact()); //폭발

        //충돌 대상이 몬스터면 피해주기
        if (collision.gameObject.tag == "Monster")
        {
            collision.gameObject.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.attackDamage * damageRate); //몬스터에게 피해주기
        }
    }
}
