using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemFunction : MonoBehaviour
{
    GameObject PlayerOnlyTransform;

    //위성
    public float satelliteRadius; //공전 반지름

    // Start is called before the first frame update
    void Start()
    {
        PlayerOnlyTransform = GameObject.Find("PlayerTransform");
    }

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
            Vector3 position = PlayerOnlyTransform.transform.position + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * satelliteRadius;
            position.y += 1.1f;

            // 오브젝트 생성 및 위치 설정
            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.SetParent(PlayerOnlyTransform.transform); // 부모 설정
        }
    }

    public void GenerateMetdolee(GameObject prefab)
    {
        Vector3 genPosition = PlayerOnlyTransform.transform.position;
        genPosition.y += 1.0f; //땅 밑으로 떨어지지 안도록 생성 위치 조정
        Instantiate(prefab, genPosition, Quaternion.identity);
    }
}
