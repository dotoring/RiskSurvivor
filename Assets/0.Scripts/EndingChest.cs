using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingChest : MonoBehaviour
{
    public GameObject chestTop;
    public float rotSpeed;
    public GameObject canvas;
    public ParticleSystem confetti;
    bool playerInRange = false;
    bool open = false;
    GameMgr gameMgr;
    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
    }

    void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E) && open == false)
        {
            open = true;
            canvas.SetActive(false);
            StartCoroutine(OpenChest());
        }
    }

    IEnumerator OpenChest()
    {
        while(chestTop.transform.rotation.eulerAngles.x < 45)
        {
            chestTop.transform.Rotate(rotSpeed * Time.deltaTime, 0f, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        confetti.Play();
        yield return new WaitForSeconds(1.0f);
        gameMgr.Ending();
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            canvas.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canvas.SetActive(false);
            playerInRange = false;
        }
    }
}
