using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
    public Transform playerTr;

    void Start()
    {
        GameObject gameObject = GameObject.Find("Player");
        playerTr = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        transform.position = playerTr.position;
    }
}
