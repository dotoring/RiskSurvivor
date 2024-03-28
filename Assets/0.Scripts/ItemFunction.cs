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

    //집중
    IEnumerator FocusCoroutine = null;
    public ParticleSystem powerUpAura;

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
    IEnumerator LaunchingMissile(GameObject prefab, int quantity)
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
    IEnumerator SprintMushroomCoroutine(int val, int quantity)
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

    //불길
    public void GenFireRoad(GameObject pref, int quantity)
    {
        //진행 중이던 코루틴이 있다면 중지
        if(fireRoadCoroutine != null)
        {
            StopCoroutine(fireRoadCoroutine);
        }
        //새 코루틴 진행
        fireRoadCoroutine = FireRoadGenCoroutine(pref, quantity);
        StartCoroutine(fireRoadCoroutine);
    }
    IEnumerator FireRoadGenCoroutine(GameObject pref, int quantity)
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

    //집중
    public void FocusOn(float val, int quantity)
    {
        //진행 중이던 코루틴이 있다면 중지
        if (FocusCoroutine != null)
        {
            StopCoroutine(FocusCoroutine);
        }
        //새 코루틴 진행
        FocusCoroutine = FocusingCoroutine(val, quantity);
        StartCoroutine(FocusCoroutine);
    }
    IEnumerator FocusingCoroutine(float val, int quantity)
    {
        float bonusRate = quantity * val; //갯수에 따른 피해증가량 계산
        bool flag = false; //피해증가 1번만 적용하기 위한 플래그
        while (true)
        {
            if (starterAssetsInputs.move == Vector2.zero) //움직이지 않으면
            {
                yield return new WaitForSeconds(0.5f);
                powerUpAura.Play(); //피해 증가 이펙트 재생
                if(flag == false)
                {
                    //피해 증가
                    PlayerValue.Instance.IncreaseDamageBonus(bonusRate);
                    flag = true;
                }
            }
            else //움직이면
            {
                powerUpAura.Stop(); //피해 증가 이펙트 중지
                if(flag == true)
                {
                    //증가 수치만큼 피해 감소
                    PlayerValue.Instance.DecreaseDamageBonus(bonusRate);
                    flag = false;
                }
            }
            yield return null;
        }
    }
}
