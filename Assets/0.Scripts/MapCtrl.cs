using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : MonoBehaviour
{
    private Vector3 lastPosition;

    public static bool flag = false;

    MapMgr mapMgr = null;
    GameMgr gameMgr = null;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        GameObject mgrGO = GameObject.Find("GameMgr");
        mapMgr = mgrGO.GetComponent<MapMgr>();
        gameMgr = mgrGO.GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 prevPosition = transform.position;

        Vector3 curPosition = transform.position;
        if (flag == true)
        {
            Vector3 moveDirection = curPosition - lastPosition;
            if (moveDirection.magnitude > 0.00f) //이동량이 일정값 이상일 때 이동한 것으로 판정
            {
                int x = (Mathf.Abs((int)(this.transform.position.x - (mapMgr.mapSize/2))) + 2) % mapMgr.mapSize;
                int z = (Mathf.Abs((int)(this.transform.position.z - (mapMgr.mapSize/2))) + 2) % mapMgr.mapSize;
                Debug.Log(x  + " " + z);

                if (x < z)
                //if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z)) // x방향으로의 이동값이 더 클 때
                {
                    if (moveDirection.x > 0) // 오른쪽으로 이동한 경우
                    {
                        Debug.Log("플레이어가 오른쪽으로 이동했습니다.");
                        mapMgr.Relocate(3);
                    }
                    else // 왼쪽으로 이동한 경우
                    {
                        Debug.Log("플레이어가 왼쪽으로 이동했습니다.");
                        mapMgr.Relocate(2);
                    }
                }
                else // z 방향으로의 이동이 더 크거나 같다면
                {
                    if (moveDirection.z > 0) // 앞쪽으로 이동한 경우
                    {
                        Debug.Log("플레이어가 위쪽으로 이동했습니다."); // 여기가 수정된 부분입니다.
                        mapMgr.Relocate(0);
                    }
                    else // 뒤쪽으로 이동한 경우
                    {
                        Debug.Log("플레이어가 아래쪽으로 이동했습니다."); // 여기가 수정된 부분입니다.
                        mapMgr.Relocate(1);
                    }
                }
            }
            flag = false;
        }

        // 이전 위치 갱신
        lastPosition = prevPosition;
    }
}
