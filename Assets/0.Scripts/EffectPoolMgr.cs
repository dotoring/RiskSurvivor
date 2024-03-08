using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolMgr : MonoBehaviour
{
    public GameObject bulletImpactPref;
    public List<GameObject> bulletObjectPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 8; i++)
        {
            GameObject go = Instantiate(bulletImpactPref, transform);
            go.SetActive(false);
            bulletObjectPool.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
