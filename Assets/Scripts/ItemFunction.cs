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

    Vector3 genPosition;

    //위성
    float satelliteRadius; //공전 반지름

    //미사일
    IEnumerator missileCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        PlayerOnlyTransform = GameObject.Find("PlayerTransform");
    }

    //위성
    public void GenerateSatellite(GameObject prefab, int quantity)
    {
        //원래 있던 위성들 전부 제거
        foreach (Transform child in PlayerOnlyTransform.transform)
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
            genPosition = PlayerOnlyTransform.transform.position + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * satelliteRadius;
            genPosition.y += 1.1f;

            // 오브젝트 생성 및 위치 설정
            GameObject obj = Instantiate(prefab, genPosition, Quaternion.identity);
            obj.transform.SetParent(PlayerOnlyTransform.transform); // 부모 설정
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
}
