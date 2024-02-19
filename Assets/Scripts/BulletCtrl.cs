using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float bulletSpeed = 1000.0f; //총알 투사체 속도

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(ThirdPersonController.shotDir * bulletSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Monster")
        {
            other.GetComponent<MonsterCtrl>().Damaged(50);
        }
        Debug.Log("bang");
        Destroy(gameObject);
    }
}
