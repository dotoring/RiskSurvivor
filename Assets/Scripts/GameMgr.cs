using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameMgr : MonoBehaviour
{
    public GameObject MonsterPref;
    GameObject player;
    public GameObject ItemSelectPanel;

    public NavMeshSurface navMeshSurface;

    public static bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정
        QualitySettings.vSyncCount = 0; //모니터 주사율 고정

        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.Find("Player");
        navMeshSurface.BuildNavMesh();

        StartCoroutine(MonsterSpawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GamePause()
    {
        Time.timeScale = 0;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ItemSelectPopUp()
    {
        GamePause();
        ItemSelectPanel.SetActive(true);
    }

    IEnumerator MonsterSpawn()
    {
        while(true)
        {
            NavMeshHit hit;
            Vector3 randomPosition = Vector3.zero;

            float randX = Random.Range(-50, 50);
            float randZ = Random.Range(-50, 50);
            Vector3 randPos = new Vector3(randX, 0, randZ);

            if (NavMesh.SamplePosition(randPos, out hit, 1.0f, NavMesh.AllAreas))
            {
                randomPosition = hit.position;
            }

            GameObject monster = Instantiate(MonsterPref);
            monster.transform.position = randomPosition;
            yield return new WaitForSeconds(3.0f);
        }
    }
}
