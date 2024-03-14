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
        if (bulletType == BulletType.explosion && impactEffect != null)
        {
            GameObject go = Instantiate(impactEffect, transform.position, Quaternion.identity);
            go.GetComponent<CheckPlayerInArea>().damage = damage;
            Destroy(go, 2.0f);
        }

        if(bulletType == BulletType.normal)
        {
            if(other.tag == "Player")
            {
                PlayerValue.Instance.PlayerTakeDamage(damage);

            }
        }
        Destroy(gameObject);
    }
}
