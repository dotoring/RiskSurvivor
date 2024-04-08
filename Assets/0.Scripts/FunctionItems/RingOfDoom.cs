using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfDoom : MonoBehaviour
{
    CheckObjectInArea check;
    public float radiusBasic;
    public float radiusIncrease;
    public float scaleIncreaseRate;
    public float lifeTime;

    void Start()
    {
        check = GetComponent<CheckObjectInArea>();
        //데미지 배율 1.5
        check.damage = PlayerValue.Instance.attackDamage * 1.5f;
        //범위 = 기본범위 + (갯수 - 1) * 증가율
        check.checkRadius = radiusBasic + (PlayerValue.Instance.ringOfDoom - 1) * radiusIncrease;

        //현재 스케일 값을 가져오기
        Vector3 currentScale = transform.localScale;
        //갯수에 따라 증가율만큼 스케일 증가
        Vector3 newScale = currentScale + (PlayerValue.Instance.ringOfDoom - 1) * Vector3.one * scaleIncreaseRate;
        //새로운 스케일 값 설정
        transform.localScale = newScale;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if( lifeTime < 0 )
        {
            Destroy(gameObject);
        }
    }
}
