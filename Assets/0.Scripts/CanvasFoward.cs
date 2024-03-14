using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFoward : MonoBehaviour
{
    public Camera mainCamera;

    private void Start()
    {
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        //transform.LookAt(mainCamera.transform.position);
    }
}
