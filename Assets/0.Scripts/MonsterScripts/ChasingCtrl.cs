using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingCtrl : MonoBehaviour
{
    Transform playerTr;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.Find("PlayerTransform").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = playerTr.position;
        targetPosition.y += 1.0f;

        //목표물 방향으로 부드럽게 회전
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8.0f * Time.deltaTime);

        //바라보는 방향으로 이동
        transform.Translate(Vector3.forward * 5 * Time.deltaTime);
    }
}
