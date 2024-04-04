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
        if (other.tag == "Monster") //몬스터 충돌시
        {
            if(other.GetComponent<MonsterCtrl>() != null) //부위 단일 개체일 경우
            {
                hitSound.Play();
                //150% 데미지
                other.GetComponent<MonsterCtrl>().Damaged(PlayerValue.Instance.DamageCalc(1.5f));
            }
            if(other.GetComponent<MonsterColliderParts>() != null) //부위별로 충돌체가 있는 경우
            {
                //해당 몬스터의 부위를 통해 몬스터컨트롤 가져오기
                MonsterCtrl mc = other.GetComponent<MonsterColliderParts>().monsterCtrl;
                if(!mc.damageApplied)
                {
                    hitSound.Play();
                    //150%데미지
                    mc.Damaged(PlayerValue.Instance.DamageCalc(1.5f));
                    //중복 피해 방지 플래그
                    mc.damageApplied = true;
                }
            }
        }
    }
}
