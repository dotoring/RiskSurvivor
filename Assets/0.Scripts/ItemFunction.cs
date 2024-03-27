using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

public class ItemFunction : MonoBehaviour
{
    GameObject PlayerOnlyTransform;
    GameObject player;
    StarterAssetsInputs starterAssetsInputs;

    Vector3 genPosition;

    //위성
    float satelliteRadius = 3.0f; //공전 반지름
    GameObject satelliteGroup;

    //미사일
    IEnumerator missileCoroutine = null;

    //질뿜버섯
    IEnumerator mushroomCoroutine = null;
    public ParticleSystem healingAura;

    //불길
    IEnumerator fireRoadCoroutine = null;
    public List<GameObject> fireEffectPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        PlayerOnlyTransform = GameObject.Find("PlayerTransform");
        satelliteGroup = GameObject.Find("SatelliteGroup");
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    //위성
    public void GenerateSatellite(GameObject prefab, int quantity)
    {
        //원래 있던 위성들 전부 제거
        foreach (Transform child in satelliteGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // 각도 간격 계산
        float angleStep = 360f / quantity;

        // 오브젝트 재생성 및 배치
        for (int i = 0; i < quantity; i++)
        {
            // 오브젝트의 각도 계산
            float angle = i * angleStep;

            // 오브젝트의 위치 계산
            genPosition = satelliteGroup.transform.position + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * satelliteRadius;
            genPosition.y += 1.1f;

            // 오브젝트 생성 및 위치 설정
            GameObject obj = Instantiate(prefab, genPosition, Quaternion.identity);
            obj.transform.SetParent(satelliteGroup.transform); // 부모 설정
        }
    }

    //멧돌이
    public void GenerateMetdolee(GameObject prefab)
    {
        genPosition = PlayerOnlyTransform.transform.position;
        genPosition.y += 1.0f; //땅 밑으로 떨어지지 안도록 생성 위치 조정
        Instantiate(prefab, genPosition, Quaternion.identity);
    }

    //미사일
    public void GenerateMissile(GameObject prefab, int quantity)
    {
        if(missileCoroutine != null) //실행중인 미사일 발사 코루틴이 있으면 종료
        {
            StopCoroutine(missileCoroutine);
        }
        missileCoroutine = LaunchingMissile(prefab, quantity); //미사일 발사 코루틴 갯수 변경
        StartCoroutine(missileCoroutine); //미사일 발사 코루틴 실행
    }
    public IEnumerator LaunchingMissile(GameObject prefab, int quantity)
    {
        while (true)
        {
            for (int i = 0; i < quantity; ++i)
            {
                //미사일 발사 위치 설정
                genPosition = player.transform.position;
                genPosition.y += 1.0f;
                genPosition.z -= 0.3f;
                //미사일 생성 및 위치 변경
                GameObject missile = Instantiate(prefab);
                missile.transform.position = genPosition;
                yield return new WaitForSeconds(0.2f); //미사일간 발사 간격
            }

            yield return new WaitForSeconds(5.0f); //미사일 발사 쿨타임
        }
    }

    //질뿜버섯
    public void ExeSprintMushroom(int val, int quantity)
    {
        if (mushroomCoroutine != null) //실행중인 질뿜버섯 코루틴이 있으면 종료
        {
            StopCoroutine(mushroomCoroutine);
        }
        mushroomCoroutine = SprintMushroomCoroutine(val, quantity); //질뿜버섯 코루틴 갯수 변경
        StartCoroutine(mushroomCoroutine); //질뿜버섯 코루틴 실행
    }
    public IEnumerator SprintMushroomCoroutine(int val, int quantity)
    {
        while(true)
        {
            if (starterAssetsInputs.sprint) //달리는 중에만
            {
                healingAura.Play();
                PlayerValue.Instance.curHp += val * quantity; //3만큼 체력 회복
                                                            //현제 체력이 최대체력보다 많아지면 최대체력으로 제한
                if (PlayerValue.Instance.curHp > PlayerValue.Instance.maxHp)
                {
                    PlayerValue.Instance.curHp = PlayerValue.Instance.maxHp;
                }
            }
            else
            {
                healingAura.Stop();
            }
            //1초당으로
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void GenFireRoad(GameObject pref, int quantiry)
    {
        //진행 중이던 코루틴이 있다면 중지
        if(fireRoadCoroutine != null)
        {
            StopCoroutine(fireRoadCoroutine);
        }
        //새 코루틴 진행
        fireRoadCoroutine = FireRoadGenCoroutine(pref, quantiry);
        StartCoroutine(fireRoadCoroutine);
    }
    public IEnumerator FireRoadGenCoroutine(GameObject pref, int quantity)
    {
        while(true)
        {
            if(starterAssetsInputs.sprint)
            {
                //오브젝트 풀링
                foreach (GameObject fireGO in fireEffectPool)
                {
                    if (!fireGO.activeSelf) //비활성화 오브젝트 찾기
                    {
                        fireGO.SetActive(true);
                        //갯수 초기화
                        fireGO.GetComponent<FireRoadCtrl>().quantity = quantity;
                        //위치 조정
                        fireGO.transform.position = PlayerOnlyTransform.transform.position;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
