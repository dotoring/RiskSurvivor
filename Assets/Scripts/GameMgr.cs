using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameMgr : MonoBehaviour
{
    public GameObject monsterPref;
    GameObject player;

    public ItemSO[] items;

    public GameObject itemSelectPanel;
    public GameObject itemSelectList;
    public GameObject itemSelectNode;

    public GameObject inventoryItemList;
    public GameObject inventoryItemNode;

    public NavMeshSurface navMeshSurface;

    public bool isPaused = false; //ThirdPersonController에서 카메라 움직임 정지용

    void Start()
    {
        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정
        QualitySettings.vSyncCount = 0; //모니터 주사율 고정

        Cursor.lockState = CursorLockMode.Locked; //커서 고정
        Cursor.visible = false; //커서 숨김

        player = GameObject.Find("Player");
        navMeshSurface.BuildNavMesh(); //NavMesh 빌드

        StartCoroutine(MonsterSpawn()); //몬스터 스폰 코루틴 시작
    }

    void Update()
    {

    }

    public void GamePause() //게임 일시정지 함수
    {
        Time.timeScale = 0; //인게임 시간 정지
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; //커서 고정 해제
        Cursor.visible = true; //커서 보이기
    }

    public void GamePlay() //게임 재진행 함수
    {
        Time.timeScale = 1; //인게임 시작 다시 흐르게
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ItemSelectPopUp() //아이템 선택창 팝업 함수
    {
        GamePause(); //게임 일시정지
        itemSelectPanel.SetActive(true); //아이템 선택창 활성화
        for(int i = 0; i < 3; i++) //선택 아이템 3개 띄우기
        {
            GameObject node = Instantiate(itemSelectNode);
            int rand = Random.Range(0, items.Length); //아이템 중 랜덤 뽑기
            node.GetComponent<ItemSelectNode>().SetItem(items[rand]); //아이템 선택 노드에 뽑은 아이템 세팅
            node.transform.SetParent(itemSelectList.transform, false); //아이템 선택 리스트의 하위 오브젝트로 설정
        }
    }

    public void ItemSelectPopDown() //아이템 선택창 비활성화 함수
    {
        foreach(Transform child in itemSelectList.transform) //아이템 선택 리스트 하위에 있는 아이템 선택 노드들 제거
        {
            Destroy(child.gameObject);
        }
        itemSelectPanel.SetActive(false); //아이템 선택창 비활성화

        RefreshInventory(); //보유 아이템 창 새로고침
    }

    public void RefreshInventory() //보유 아이템 창 새로고침 함수
    {
        foreach(Transform child in inventoryItemList.transform) //보유 아이템 리스트의 오브젝트들 제거
        {
            Destroy(child.gameObject);
        }

        foreach(ItemSO item in Inventory.itemList) //보유 아이템 리스트에 오브젝트들 다시 생성
        {
            GameObject node = Instantiate(inventoryItemNode);
            node.GetComponent<InventoryItemNode>().SetItem(item);
            node.transform.SetParent(inventoryItemList.transform, false);
        }
    }

    IEnumerator MonsterSpawn() //몬스터 스폰 코루틴함수
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

            GameObject monster = Instantiate(monsterPref);
            monster.transform.position = randomPosition;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
