using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float bulletSpeed = 1000.0f; //총알 투사체 속도
    public float bulletLifeTime = 10.0f; //총알 지속 시간

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(ThirdPersonController.shotDir * bulletSpeed);
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
        if(other.tag == "Monster")
        {
            other.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.DamageCalc());
        }
        Destroy(gameObject);
    }

}
