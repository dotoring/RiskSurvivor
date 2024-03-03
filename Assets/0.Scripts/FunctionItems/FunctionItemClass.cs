using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionItemClass : MonoBehaviour
{
    protected GameMgr gameMgr;

    protected Transform GetClosestMonster(float trackingRange) //가장 가까이 있는 몬스터 탐색 함수 (탐색범위)
    {
        Transform closestMonster = null;
        float closestDistanceSqr = Mathf.Infinity;

        // 모든 몬스터들의 위치를 확인하고 가장 가까운 몬스터 선택
        foreach (Transform monster in gameMgr.monstersTr)
        {
            Vector3 directionToTarget = monster.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            // 추적 범위 안에 있는 몬스터인지 확인, 가장 가까운 몬스터인지 확인
            if (dSqrToTarget < trackingRange * trackingRange && dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget; //가장 가까운 거리 재설정
                closestMonster = monster; //가장 가까운 몬스터 설정
            }
        }

        return closestMonster;
    }
}
