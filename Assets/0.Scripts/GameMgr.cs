using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurface;

    public float playTime;
    public Text timeText;
    GameObject player;

    [Header("Pause")]
    public bool isPaused = false; //ThirdPersonController에서 카메라 움직임 정지용
    public GameObject pausePanel;
    public Button continueBtn;
    public Button menuBtn;
    public GameObject gameoverPanel;
    public Text resultText;
    public Button restartBtn;
    public Button menuBtn2;

    [Header("Item")]
    public ItemSO[] items;
    int[] temp = new int[3];

    public GameObject itemSelectPanel;
    public GameObject itemSelectList;
    public GameObject itemSelectNode;

    public GameObject inventoryItemList;
    public GameObject inventoryItemNode;

    [Header("Monster")]
    public GameObject smallMonsterPref;
    public GameObject mediumMonsterPref;
    public GameObject flyMonsterPref;
    public List<Transform> monstersTr = new List<Transform>(); //몬스터들의 위치 리스트(추적용)
    //오브젝트 풀
    public List<GameObject> smallMonsterPool = new List<GameObject>(); //몬스터 풀
    public List<GameObject> mediumMonsterPool = new List<GameObject>(); //몬스터 풀
    public List<GameObject> flyMonsterPool = new List<GameObject>(); //몬스터 풀
    public Inventory inventory;

    private void Awake()
    {
        foreach (NavMeshSurface surface in navMeshSurface)
        {
            surface.BuildNavMesh(); //NavMesh 빌드
        }
    }
    void Start()
    {
        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정
        QualitySettings.vSyncCount = 0; //모니터 주사율 고정

        Time.timeScale = 1; //인게임 시간 흐르게
        Cursor.lockState = CursorLockMode.Locked; //커서 고정
        Cursor.visible = false; //커서 숨김

        pausePanel.SetActive(false);

        player = GameObject.Find("Player");
        playTime = 0;

        StartCoroutine(MonsterSpawn()); //몬스터 스폰 코루틴 시작
        StartCoroutine(MediumMonsterSpawn());
        StartCoroutine(FlyMonsterSpawn());

        continueBtn.onClick.AddListener(() =>
        {
            pausePanel.SetActive(false);
            GamePlay();
        });
        menuBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenuScene");
        });

        restartBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("PlayScene");
        });
        menuBtn2.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenuScene");
        });
    }

    void Update()
    {
        playTime += Time.deltaTime;
        Clock();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause();
            pausePanel.SetActive(true);
        }

        if(playTime >= 60*10)
        {
            GamePause();
            resultText.text = "이김";
            gameoverPanel.SetActive(true);
        }

        if (PlayerValue.Instance.curHp <= 0)
        {
            GamePause();
            resultText.text = "죽음";
            gameoverPanel.SetActive(true);
        }
    }

    void Clock()
    {
        int minutes = (int)playTime / 60;
        int seconds = (int)playTime % 60;
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
        Time.timeScale = 1; //인게임 시간 다시 흐르게
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ItemSelectPopUp() //아이템 선택창 팝업 함수
    {
        GamePause(); //게임 일시정지
        itemSelectPanel.SetActive(true); //아이템 선택창 활성화

        for (int i = 0; i < 3;) //중복 없이 아이템 3개 뽑기
        {
            int ran = Random.Range(0, items.Length);
            if (temp.Contains(ran))
            {
                continue;
            }
            else
            {
                temp[i] = ran;
                i++;
            }
        }

        for (int i = 0; i < 3; i++) //선택 아이템 3개 띄우기
        {
            GameObject node = Instantiate(itemSelectNode);
            node.GetComponent<ItemSelectNode>().SetItem(items[temp[i]]); //아이템 선택 노드에 뽑은 아이템 세팅
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

        foreach(ItemSO item in inventory.itemList) //보유 아이템 리스트에 오브젝트들 다시 생성
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
            int spawnCount = 3 + ((int)playTime / 60);
            for (int i = 0; i < spawnCount; i++)
            {
                NavMeshHit hit;
                Vector3 randomPosition = Vector3.zero;
                //플레이어 반경 30이내의 랜덤위치 설정
                Vector3 randPos = player.transform.position + Random.insideUnitSphere * 50.0f;

                //랜덤 위치에서 가장 가까운 정상 지형 찾기
                if (NavMesh.SamplePosition(randPos, out hit, 100.0f, NavMesh.AllAreas))
                {
                    randomPosition = hit.position;
                }

                bool poolAvailable = false;
                //몬스터 오브젝트 풀에서 가져오기
                foreach (GameObject monsterGO in smallMonsterPool)
                {
                    if (!monsterGO.activeSelf) //비활성화 몬스터라면
                    {
                        monsterGO.SetActive(true);
                        monsterGO.transform.position = randomPosition; //몬스터 스폰 위치 조정
                        monstersTr.Add(monsterGO.transform); //몬스터 위치 리스트에 추가
                        poolAvailable = true;
                        break;
                    }
                }
                //몬스터 풀에 자원이 없으면 추가 생성
                if (!poolAvailable)
                {
                    GameObject monster = Instantiate(smallMonsterPref); //몬스터 스폰
                    monster.transform.position = randomPosition; //몬스터 스폰 위치 조정
                    smallMonsterPool.Add(monster);
                    monstersTr.Add(monster.transform); //몬스터 위치 리스트에 추가
                }
            }
            yield return new WaitForSeconds(8.0f); //스폰 쿨타임
        }
    }

    IEnumerator MediumMonsterSpawn() //몬스터 스폰 코루틴함수
    {
        while (true)
        {
            yield return new WaitForSeconds(35.0f); //스폰 쿨타임

            int spawnCount = 1 + ((int)playTime / 120);
            for (int i = 0; i < spawnCount; i++)
            {
                NavMeshHit hit;
                Vector3 randomPosition = Vector3.zero;
                //플레이어 반경 30이내의 랜덤위치 설정
                Vector3 randPos = player.transform.position + Random.insideUnitSphere * 50.0f;

                //랜덤 위치에서 가장 가까운 정상 지형 찾기
                if (NavMesh.SamplePosition(randPos, out hit, 100.0f, NavMesh.AllAreas))
                {
                    randomPosition = hit.position;
                }

                bool poolAvailable = false;
                //몬스터 오브젝트 풀에서 가져오기
                foreach (GameObject monsterGO in mediumMonsterPool)
                {
                    if (!monsterGO.activeSelf) //비활성화 몬스터라면
                    {
                        monsterGO.SetActive(true);
                        monsterGO.transform.position = randomPosition; //몬스터 스폰 위치 조정
                        monstersTr.Add(monsterGO.transform); //몬스터 위치 리스트에 추가
                        poolAvailable = true;
                        break;
                    }
                }
                //몬스터 풀에 자원이 없으면 추가 생성
                if (!poolAvailable)
                {
                    GameObject monster = Instantiate(mediumMonsterPref); //몬스터 스폰
                    monster.transform.position = randomPosition; //몬스터 스폰 위치 조정
                    mediumMonsterPool.Add(monster);
                    monstersTr.Add(monster.transform); //몬스터 위치 리스트에 추가
                }
            }
        }
    }

    IEnumerator FlyMonsterSpawn() //몬스터 스폰 코루틴함수
    {
        while (true)
        {
            yield return new WaitForSeconds(15.0f); //스폰 쿨타임

            int spawnCount = 2 + ((int)playTime / 120);
            for (int i = 0; i < spawnCount; i++)
            {
                NavMeshHit hit;
                Vector3 randomPosition = Vector3.zero;
                //플레이어 반경 30이내의 랜덤위치 설정
                Vector3 randPos = player.transform.position + Random.insideUnitSphere * 50.0f;

                //랜덤 위치에서 가장 가까운 정상 지형 찾기
                if (NavMesh.SamplePosition(randPos, out hit, 100.0f, NavMesh.AllAreas))
                {
                    randomPosition = hit.position;
                }

                randomPosition.y += Random.Range(1.0f, 4.0f);

                bool poolAvailable = false;
                //몬스터 오브젝트 풀에서 가져오기
                foreach (GameObject monsterGO in flyMonsterPool)
                {
                    if (!monsterGO.activeSelf) //비활성화 몬스터라면
                    {
                        monsterGO.SetActive(true);
                        monsterGO.transform.position = randomPosition; //몬스터 스폰 위치 조정
                        monstersTr.Add(monsterGO.transform); //몬스터 위치 리스트에 추가
                        poolAvailable = true;
                        break;
                    }
                }
                //몬스터 풀에 자원이 없으면 추가 생성
                if (!poolAvailable)
                {
                    GameObject monster = Instantiate(flyMonsterPref); //몬스터 스폰
                    monster.transform.position = randomPosition; //몬스터 스폰 위치 조정
                    flyMonsterPool.Add(monster);
                    monstersTr.Add(monster.transform); //몬스터 위치 리스트에 추가
                }
            }
        }
    }
}
