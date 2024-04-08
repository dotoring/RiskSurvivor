using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    public GameObject playerRagdoll;
    bool isGameOver = false;

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
    public ItemSO selectedItem;
    public Button itemDecideBtn;

    public GameObject inventoryItemList;
    public GameObject inventoryItemNode;

    [Header("Monster")]
    public GameObject smallMonsterPref;
    public GameObject mediumMonsterPref;
    public GameObject flyMonsterPref;
    public GameObject ghostGroupPref;
    public GameObject bossMonsterPref;
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

        StartCoroutine(SmallMonsterSpawn()); //몬스터 스폰 코루틴 시작
        StartCoroutine(MediumMonsterSpawn());
        StartCoroutine(FlyMonsterSpawn());
        StartCoroutine(GhostGroupSpawn());

        continueBtn.onClick.AddListener(() =>
        {
            pausePanel.SetActive(false);
            if(!itemSelectPanel.activeSelf) //아이템 선택 중이 아니면
            {
                GamePlay();
            }
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
        itemDecideBtn.onClick.AddListener(() =>
        {
            if(selectedItem != null)
            {
                ItemSelect(selectedItem);
            }
        });
    }

    void Update()
    {
        if(!isGameOver)
        {
            playTime += Time.deltaTime;
        }
        Clock();

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
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

        if (PlayerValue.Instance.curHp <= 0 && !isGameOver)
        {
            isGameOver = true;
            StartCoroutine(GameOver());
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

    IEnumerator GameOver()
    {
        //UI변화를 위해 한프레임 기다리기
        yield return new WaitForSeconds(0.02f);

        //플레이어 랙돌 생성
        Instantiate(playerRagdoll, player.transform.position, player.transform.rotation);
        //기존 플레이어 비활성화
        player.SetActive(false);
        
        //3초 뒤 게임오버 화면 띄우기
        yield return new WaitForSeconds(3.0f);
        GamePause();
        resultText.text = "죽음";
        gameoverPanel.SetActive(true);

        yield break;
    }

    public void ItemSelectPopUp() //아이템 선택창 팝업 함수
    {
        GamePause(); //게임 일시정지
        itemSelectPanel.SetActive(true); //아이템 선택창 활성화

        for (int i = 0; i < 3;) //중복 없이 아이템 3개 뽑기
        {
            int ran;
            //아이템 등급 정하기
            int grade = Random.Range(1, 101);
            Debug.Log(grade);
            if(grade <= 75) //일반
            {
                Debug.Log("일반");
                while(true) //일반 아이템을 뽑을 때까지 반복
                {
                    ran = Random.Range(0, items.Length);
                    if (items[ran].itemGrade == ItemGrade.nomal) //일반 아이템이면 통과
                    {
                        if (temp.Contains(ran)) //중복이면 다시
                        {
                            continue;
                        }
                        else //중복되지 않은 아이템이면 통과
                        {
                            break;
                        }
                    }
                }
            }
            else if(grade > 95) //전설
            {
                Debug.Log("전설");
                while (true) //전설 아이템을 뽑을 때까지 반복
                {
                    ran = Random.Range(0, items.Length);
                    if (items[ran].itemGrade == ItemGrade.legend) //전설 아이템이면 통과
                    {
                        if (temp.Contains(ran)) //중복이면 다시
                        {
                            continue;
                        }
                        else //중복되지 않은 아이템이면 통과
                        {
                            break;
                        }
                    }
                }
            }
            else //에픽
            {
                Debug.Log("에픽");
                while (true) //에픽 아이템을 뽑을 때까지 반복
                {
                    ran = Random.Range(0, items.Length);
                    if (items[ran].itemGrade == ItemGrade.epic) //에픽 아이템이면 통과
                    {
                        if (temp.Contains(ran)) //중복이면 다시
                        {
                            continue;
                        }
                        else //중복되지 않은 아이템이면 통과
                        {
                            break;
                        }
                    }
                }
            }

            temp[i] = ran;
            i++;
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

    public void ItemSelect(ItemSO item)
    {
        inventory.AddItem(item); //인벤토리에 아이템 추가
        ItemSelectPopDown(); //아이템 선택창 비활성화
        GamePlay(); //게임 다시진행
        selectedItem = null;
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

    IEnumerator SmallMonsterSpawn() //소형 몬스터 스폰 코루틴함수
    {
        while(true)
        {
            int spawnCount = 3 + ((int)playTime / 90);
            for (int i = 0; i < spawnCount; i++)
            {
                NavMeshHit hit;
                Vector3 randomPosition = Vector3.zero;
                //플레이어 반경 30이내의 랜덤위치 설정
                Vector3 randPos = Random.insideUnitSphere * 100.0f;

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

    IEnumerator MediumMonsterSpawn() //중형 몬스터 스폰 코루틴함수
    {
        while (true)
        {
            yield return new WaitForSeconds(35.0f); //스폰 쿨타임

            int spawnCount = 1 + ((int)playTime / 150);
            for (int i = 0; i < spawnCount; i++)
            {
                NavMeshHit hit;
                Vector3 randomPosition = Vector3.zero;
                //플레이어 반경 30이내의 랜덤위치 설정
                Vector3 randPos = Random.insideUnitSphere * 100.0f;

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

    IEnumerator FlyMonsterSpawn() //공중 몬스터 스폰 코루틴함수
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
                Vector3 randPos = Random.insideUnitSphere * 100.0f;

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

    IEnumerator GhostGroupSpawn()
    {
        yield return new WaitForSeconds(30.0f); //첫 스폰 쿨타임

        while (true)
        {
            int spawnCount = 1 + ((int)playTime / 240);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPos;

                //스폰 위치 설정 중앙제외 8방향중 한곳으로
                int[] x = { -1, 0, 1 };
                int[] z = { -1, 0, 1 };
                while(true)
                {
                    int randX = Random.Range(0, x.Length);
                    int randZ = Random.Range(0, z.Length);
                    if(randX == 1 && randZ == 1)
                    {
                        continue;
                    }
                    else
                    {
                        spawnPos = new Vector3(x[randX], 0, z[randZ]);
                        break;
                    }
                }
                //맵 밖에서 스폰되도록 맵 크기만큼 곱하기
                spawnPos = spawnPos.normalized * 125;

                GameObject monster = Instantiate(ghostGroupPref);
                monster.transform.position = spawnPos;
            }

            yield return new WaitForSeconds(60.0f); //스폰 쿨타임
        }
    }

    IEnumerator BossMonsterSpawn()
    {
        NavMeshHit hit;
        Vector3 randomPosition = Vector3.zero;
        //플레이어 반경 30이내의 랜덤위치 설정
        Vector3 randPos = Random.insideUnitSphere * 100.0f;

        //랜덤 위치에서 가장 가까운 정상 지형 찾기
        if (NavMesh.SamplePosition(randPos, out hit, 100.0f, NavMesh.AllAreas))
        {
            randomPosition = hit.position;
        }

        GameObject monster = Instantiate(bossMonsterPref); //몬스터 스폰
        monster.transform.position = randomPosition; //몬스터 스폰 위치 조정
        flyMonsterPool.Add(monster);
        monstersTr.Add(monster.transform); //몬스터 위치 리스트에 추가

        yield return null;
    }
}
