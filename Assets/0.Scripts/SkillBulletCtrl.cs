using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBulletCtrl : MonoBehaviour
{
    public float bulletSpeed = 1000.0f; //총알 투사체 속도
    public float bulletLifeTime = 10.0f; //총알 지속 시간
    public AudioSource hitSound;

    void Start()
    {
        GameObject go = GameObject.Find("EffectPool");
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            hitSound.Play();
            other.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.DamageCalc(1.5f));
        }
    }
}
