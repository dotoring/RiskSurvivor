using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerInArea : MonoBehaviour
{
    public LayerMask playerLayer; // 플레이어 레이어
    public float checkRadius = 1.4f; // 체크할 범위 반경
    public float damage;

    void Start()
    {

        // 구형 영역 내에 있는 플레이어 검사
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, playerLayer);

        if (colliders.Length > 0)
        {
            PlayerValue.Instance.PlayerTakeDamage(damage); //플레이어에게 피해 입히기
        }
    }
}
