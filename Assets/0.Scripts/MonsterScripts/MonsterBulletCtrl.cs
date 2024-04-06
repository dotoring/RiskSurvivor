using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    normal,
    explosion,
}

public class MonsterBulletCtrl : MonoBehaviour
{
    public float bulletLifeTime = 10.0f; //총알 지속 시간
    public float damage;
    public GameObject impactEffect;
    public BulletType bulletType;

    void Start()
    {
        StartCoroutine(DestroyTime());
    }

    //10초 후에는 총알 삭제
    IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(gameObject);
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        //폭발형일 경우
        if (bulletType == BulletType.explosion && impactEffect != null)
        {
            //폭발 이펙트 생성
            GameObject go = Instantiate(impactEffect, transform.position, Quaternion.identity);
            //폭발 데미지 설정
            go.GetComponent<CheckObjectInArea>().damage = damage;
            //2초 뒤 제거
            Destroy(go, 2.0f);
        }
        else if(bulletType == BulletType.normal) //일반형 일경우
        {
            if(impactEffect != null)
            {
                //타격 이펙트 생성
                GameObject go = Instantiate(impactEffect, transform.position, Quaternion.identity);
                //2초 뒤 제거
                Destroy(go, 2.0f);
            }
            if (other.tag == "Player") //플레이어가 맞았을 경우
            {
                //데미지 주기
                PlayerValue.Instance.PlayerTakeDamage(damage);
            }
        }
        //충돌 되면 삭제
        Destroy(gameObject);
    }
}
