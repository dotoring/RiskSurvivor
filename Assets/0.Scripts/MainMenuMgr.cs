using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuMgr : MonoBehaviour
{
    public Button startBtn;
    public Button quitBtn;

    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("PlayScene");
        });

        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
