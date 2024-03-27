using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoadCtrl : FunctionItemClass
{
    public float damage; //µ¥¹ÌÁö
    public float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            lifeTime -= Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            float dmg = damage + ((damage * 0.2f) * (int)(gameMgr.playTime / 60));
            other.GetComponent<MonsterCtrl>().Damaged(dmg * Time.deltaTime);
        }
    }
}
