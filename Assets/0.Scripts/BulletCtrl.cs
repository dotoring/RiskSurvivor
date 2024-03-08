using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float bulletSpeed = 1000.0f; //총알 투사체 속도
    public float bulletLifeTime = 10.0f; //총알 지속 시간

    public GameObject impactEffect;

    EffectPoolMgr poolMgr;
    void Start()
    {
        GameObject go = GameObject.Find("EffectPool");
        poolMgr = go.GetComponent<EffectPoolMgr>();
        GetComponent<Rigidbody>().AddForce(ThirdPersonController.shotDir * bulletSpeed);
        StartCoroutine(LifeTime());
    }

    //10초 후에는 총알 삭제
    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(gameObject);
        yield return null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 impactPoint = contact.point;
        foreach(GameObject effect in poolMgr.bulletObjectPool)
        {
            if(effect.activeSelf == false)
            {
                effect.transform.position = impactPoint;
                effect.SetActive(true);
                break;
            }
        }

        if (collision.gameObject.tag == "Monster")
        {
            collision.gameObject.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.DamageCalc()); //몬스터에게 피해주기
        }
        Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Monster")
    //    {
    //        other.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.DamageCalc());
    //    }
    //    Destroy(gameObject);
    //}
}
