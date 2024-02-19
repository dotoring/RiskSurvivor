using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public GameObject MonsterPref;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MonsterSpawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator MonsterSpawn()
    {
        while(true)
        {
            Instantiate(MonsterPref);
            yield return new WaitForSeconds(3.0f);
        }
    }
}
