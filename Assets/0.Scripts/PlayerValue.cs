using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValue : MonoBehaviour
{
    public static PlayerValue Instance; //싱글턴

    //레벨 관련
    public int level; //레벨
    public int maxExp; //경험치 통
    public int curExp; //현재 경험치

    //체력 관련
    public int maxHp; //최대 체력
    public float curHp; //현재 체력
    public float hpRegen; //체력 재생
    public float hpRegenTimeout; //체력 재생 쿨타임
    public int hpGrowth; //성장 체력
    public float hpRegenTimeoutDelta;

    //방어 관련
    public int block; //방어력

    //공격 관련
    public int attackDamage; //공격력
    public float basicAttackSpeed; //기본 공격속도
    public float attackSpeedIncreaseRate; //공격속도 증가율(합연산을 위한 변수)
    public float attackSpeed; //공격 속도
    public int critChance; //치명타 확률
    public float critDmgRate; //치명타 배율

    //이동 관련
    public float basicMoveSpeed; //기본 이동속도
    public float MoveSpeedIncreaseRate; //이동속도 증가율(합연산을 위한 변수)
    public float moveSpeed; //이동 속도
    public float sprintSpeedIncreaseRate; //달리기 속도 증가율(합연산을 위한 변수)
    public float sprintSpeed; //달리기 속도
    public float jumpHeight; //점프 높이
    public int jumpCount; //점프 횟수

    [Header("UI Objects")]
    public Text levelTxt;
    public Image ExpBar;
    public Text HpTxt;
    public Image HpBar;

    ThirdPersonController controller;
    GameMgr gameMgr;

    //임시용
    ItemFunction itemFunction;

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
        itemFunction = GetComponent<ItemFunction>();

        //============게임 시작시 스탯 초기화============
        //레벨 관련
        level = 1;
        maxExp = 100;
        curExp = 0;
        
        //체력 관련
        maxHp = 200;
        curHp = maxHp;
        hpRegen = 0.5f;
        hpRegenTimeout = 1.0f;
        hpGrowth = 10;
        hpRegenTimeoutDelta = hpRegenTimeout;

        //방어 관련
        block = 0;

        //공격 관련
        attackDamage = 20;
        basicAttackSpeed = 2;
        attackSpeed = basicAttackSpeed;
        attackSpeedIncreaseRate = 1.0f;
        critChance = 0;
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
        HpRegen();
        RefreshHp();
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
        maxExp += 50; //필요 경험치량 증가
        ExpBar.fillAmount = (float)curExp / (float)maxExp; //경험치바의 게이지 변경

        IncreaseMaxHp(hpGrowth); //성장체력만큼 체력 증가
    }

    void HpRegen()
    {
        if(hpRegenTimeoutDelta <= 0.0f)
        {
            curHp += hpRegen;
            if(curHp > maxHp)
            {
                curHp = maxHp;
            }
            hpRegenTimeoutDelta = hpRegenTimeout;
        }

        if(hpRegenTimeoutDelta > 0.0f)
        {
            hpRegenTimeoutDelta -= Time.deltaTime;
        }
    }

    void RefreshHp()
    {
        HpTxt.text = curHp.ToString("F0") + "/" + maxHp.ToString();
        HpBar.fillAmount = curHp / (float)maxHp;
    }

    //===================================================================
    //====================플레이어 스탯 증가 함수들========================
    public void IncreaseMaxHp(int val) //최대 체력 증가
    {
        maxHp += val;
        curHp += val; //최대 체력 증가량 만큼 현재 체력도 증가
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

    public void IncreaseCritChance(int val)
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

    //=============데미지 계산================
    public int DamageCalc() //플레이어가 주는 데미지 계산
    {
        float dmg = attackDamage;

        if(critChance >= 100) //치명타 확률이 100%를 넘겼을 때 확정 치명타
        {
            dmg = attackDamage * critDmgRate;
        }
        else if(critChance <= 0) //치명타 확률이 0%일 때
        {
            dmg = attackDamage;
        }
        else //치명타 확률이 1%~99%일 때
        {
            int rand = Random.Range(1, 101); //1~100 랜덤
            if(rand <= critChance) //치명타가 떴을 때
            {
                dmg = attackDamage * critDmgRate;
            }
            else
            {
                dmg = attackDamage;
            }
        }

        return (int)dmg;
    }

    public void PlayerTakeDamage(int dmg) //플레이어가 받는 데미지
    {
        curHp -= dmg;
    }

    //==================기능 아이템=====================
    public void Satellite(GameObject prefab, int val)
    {
        itemFunction.GenerateSatellite(prefab, val);
    }

    public void Metdolee(GameObject prefab)
    {
        itemFunction.GenerateMetdolee(prefab);
    }

    public void Missile(GameObject prefab, int quantity)
    {
        itemFunction.GenerateMissile(prefab, quantity);
    }
}