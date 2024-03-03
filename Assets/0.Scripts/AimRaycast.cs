using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimRaycast : MonoBehaviour
{
    RaycastHit hit;
    public static Vector3 targetPoint;

    void Start()
    {
        
    }

    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            //플레이어가 조준한 곳을 타겟좌표로 설정
            targetPoint = hit.point;
        }
    }
}
