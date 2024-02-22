using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValue : MonoBehaviour
{
    public static PlayerValue Instance; //싱글턴

    //레벨 관련
    public int level;
    public int maxExp;
    public int curExp;

    //체력 관련
    public int maxHp;
    public float curHp;
    public float hpRegen;
    public int hpGrowth;

    //방어 관련
    public int block;

    //공격 관련
    public int attackDamage;
    public float basicAttackSpeed;
    public float attackSpeedIncreaseRate;
    public float attackSpeed;
    public float critChance;
    public float critDmgRate;

    //이동 관련
    public float basicMoveSpeed;
    public float MoveSpeedIncreaseRate;
    public float moveSpeed;
    public float sprintSpeedIncreaseRate;
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

    private void Awake()
    {
        //싱글턴 설정
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();

        //============게임 시작시 스탯 초기화============
        //레벨 관련
        level = 1;
        maxExp = 100;
        curExp = 0;
        
        //체력 관련
        maxHp = 500;
        curHp = maxHp;
        hpRegen = 0.5f;
        hpGrowth = 10;

        //방어 관련
        block = 0;

        //공격 관련
        attackDamage = 10;
        basicAttackSpeed = 2;
        attackSpeed = basicAttackSpeed;
        attackSpeedIncreaseRate = 1.0f;
        critChance = 0.0f;
        critDmgRate = 1.5f;

        //이동 관련
        basicMoveSpeed = controller.MoveSpeed;
        moveSpeed = controller.MoveSpeed;
        MoveSpeedIncreaseRate = 1.0f;
        sprintSpeed = controller.SprintSpeed;
        sprintSpeedIncreaseRate = 1.0f;
        jumpHeight = controller.JumpHeight;
        jumpCount = 1;

        //===============UI 초기화===================
        levelTxt.text = "Lv." + level.ToString();
        ExpBar.fillAmount = 0;
        HpBar.fillAmount = 1;
        HpTxt.text = curHp.ToString() + "/" + maxHp.ToString();
    }

    void Update()
    {

    }

    public void GainExp(int value) //경험치 획득 함수
    {
        curExp += value; //경험치 추가
        ExpBar.fillAmount = (float)curExp / (float)maxExp; //경험치바의 게이지 변경
        if(curExp >= maxExp) //경험치가 최대 경험치를 넘으면(레벨업)
        {
            int restExp = curExp - maxExp; //경험치 초과량 저장
            LevelUp(restExp);
        }
    }

    void LevelUp(int restExp) //레벨업 시 작동 함수
    {
        gameMgr.ItemSelectPopUp(); //아이템 선택 창 팝업
        level++; //레벨 증가
        levelTxt.text = "Lv." + level.ToString(); //레벨 텍스트 변경
        curExp = restExp; //경험치 초과량 채우기
        maxExp += 100; //필요 경험치량 증가
        ExpBar.fillAmount = (float)curExp / (float)maxExp; //경험치바의 게이지 변경
    }

    void RefreshHp()
    {
        HpTxt.text = curHp.ToString() + "/" + maxHp.ToString();
    }

    //===================================================================
    //====================플레이어 스탯 증가 함수들========================
    public void IncreaseMaxHp(int val) //최대 체력 증가
    {
        maxHp += val;
        curHp += val; //최대 체력 증가량 만큼 현재 체력도 증가
        RefreshHp();
    }

    public void DecreaseMaxHp()
    {

    }

    public void IncreaseHpRegen(float val) //체력 재생 증가
    {
        hpRegen += val;
    }

    public void IncreaseAttackSpeed(float val) //공격 속도 증가
    {
        attackSpeedIncreaseRate += val;
        attackSpeed = basicAttackSpeed * attackSpeedIncreaseRate;
        controller.shotTimeout = 1 / attackSpeed;
    }

    public void DecreaseAttackSpeed()
    {

    }

    public void IncreaseCritChance(float val)
    {
        critChance += val;
    }

    public void IncreaseMoveSpeed(float val)
    {
        MoveSpeedIncreaseRate += val;
        moveSpeed = basicMoveSpeed * MoveSpeedIncreaseRate;
        sprintSpeed = (moveSpeed + 2.0f) * sprintSpeedIncreaseRate;
        controller.SprintSpeed = sprintSpeed;
        controller.MoveSpeed = moveSpeed;
    }

    public void DecreaseMoveSpeed()
    {

    }

    public void IncreaseSprintSpeed(float val)
    {
        sprintSpeedIncreaseRate += val;
        sprintSpeed = (moveSpeed + 2.0f) * sprintSpeedIncreaseRate;
        controller.SprintSpeed = sprintSpeed;
    }

    public void DecreaseSprintSpeed()
    {

    }

    public void IncreaseJumpCount(int val)
    {
        jumpCount += val;
    }
}