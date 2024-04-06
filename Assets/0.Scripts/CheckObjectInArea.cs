using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum From
{
    Player,
    Monster
}

public class CheckObjectInArea : MonoBehaviour
{
    public From from;
    public LayerMask ObjectLayer; // 플레이어 레이어
    public float checkRadius; // 체크할 범위 반경
    public float damage;

    void Start()
    {

        // 구형 영역 내에 있는 플레이어 검사
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, ObjectLayer);

        if (colliders.Length > 0)
        {
            if(from == From.Monster) //몬스터의 공격이면
            {
                PlayerValue.Instance.PlayerTakeDamage(damage); //플레이어에게 피해 입히기
            }
            else if(from == From.Player) //플레이어의 공격이면
            {
                //중복 방지를 위한 세트 생성
                HashSet<GameObject> monsters = new HashSet<GameObject>();
                //감지된 콜라이더들의 몬스터를 세트에 추가
                foreach (Collider collider in colliders)
                {
                    monsters.Add(collider.gameObject);
                }
                //감지된 몬스터들에게 피해 입히기
                foreach (GameObject mon in monsters)
                {
                    mon.GetComponent<MonsterCtrl>().Damaged(damage);
                }
            }
        }
    }
}
