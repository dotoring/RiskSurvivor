using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float bulletSpeed = 1000.0f; //총알 투사체 속도

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(ThirdPersonController.shotDir * bulletSpeed);
    }

    void Update()
    {
        
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
