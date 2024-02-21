using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValue : MonoBehaviour
{
    public int level;
    public int maxExp;
    public int curExp;
    public int maxHp;
    public float curHp;
    public float hpRegen;
    public float hpGrowth;
    public int block;
    public int attackDamage;
    public int attackSpeed;
    public float critChance;
    public float critDmgRate;
    public float moveSpeed;
    public float sprintSpeed;
    public float jumpHeight;
    public int jumpCount;

    [Header("UI Objects")]
    public Text levelTxt;
    public Image ExpBar;
    public Text HpTxt;
    public Image HpBar;

    ThirdPersonController controller;
    GameMgr gameMgr;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();

        //게임 시작시 스탯 초기화
        level = 1;
        maxExp = 100;
        curExp = 0;
        maxHp = 500;
        curHp = maxHp;
        hpRegen = 0.5f;
        block = 0;
        attackDamage = 10;
        attackSpeed = 1;
        critChance = 0.0f;
        critDmgRate = 1.5f;
        moveSpeed = controller.MoveSpeed;
        sprintSpeed = controller.SprintSpeed;
        jumpHeight = controller.JumpHeight;
        jumpCount = 1;

        levelTxt.text = "Lv." + level.ToString();
        ExpBar.fillAmount = 0;
        HpBar.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshStat()
    {
        //maxHp = maxHp + (hpGrowth * level) + (item * itemCount);
        //block = block + item * itemCount;
    }

    public void GainExp(int value)
    {
        curExp += value;
        ExpBar.fillAmount = (float)curExp / (float)maxExp;
        if(curExp >= maxExp)
        {
            int restExp = curExp - maxExp;
            LevelUp(restExp);
        }
    }

    void LevelUp(int restExp)
    {
        gameMgr.ItemSelectPopUp();
        level++;
        levelTxt.text = "Lv." + level.ToString();
        curExp = restExp;
        maxExp += 100;
        ExpBar.fillAmount = (float)curExp / (float)maxExp;
    }

    void RefreshHp()
    {
        
    }
}