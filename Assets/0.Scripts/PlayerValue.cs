using Micosmo.SensorToolkit;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValue : MonoBehaviour
{
    public static PlayerValue Instance; //싱글턴

    //레벨 관련
    [HideInInspector] public int level; //레벨
    public int maxExp; //경험치 통
    [HideInInspector] public int curExp; //현재 경험치

    //체력 관련
    public int maxHp; //최대 체력
    public float curHp; //현재 체력
    public float hpRegen; //체력 재생
    [HideInInspector] public float hpRegenTimeout; //체력 재생 쿨타임
    public int hpGrowth; //성장 체력
    [HideInInspector] public float hpRegenTimeoutDelta;

    //방어 관련
    public int block; //방어력
    public int maxBarrier; //최대 보호막
    public float curBarrier; //현재 보호막

    //공격 관련
    public float attackDamage; //공격력
    public float attackDamageGrowth; //성장 공격력
    public float attackDamageBonusRate; //공격력 추가 배율
    public float basicAttackSpeed; //기본 공격속도
    [HideInInspector] public float attackSpeedIncreaseRate; //공격속도 증가율(합연산을 위한 변수)
    [HideInInspector] public float attackSpeed; //공격 속도
    public int critChance; //치명타 확률
    public float critDmgRate; //치명타 배율

    //이동 관련
    public float basicMoveSpeed; //기본 이동속도
    [HideInInspector] public float MoveSpeedIncreaseRate; //이동속도 증가율(합연산을 위한 변수)
    public float moveSpeed; //이동 속도
    [HideInInspector] public float sprintSpeedIncreaseRate; //달리기 속도 증가율(합연산을 위한 변수)
    public float sprintSpeed; //달리기 속도
    public float jumpHeight; //점프 높이
    public int jumpCount; //점프 횟수

    //스킬 관련
    public int skillMaxCount;
    public int skillCount;

    [Header("Items")]
    public int leechingSeed;
    public int vampiricTooth;
    public float luckyShotRate;
    public ItemSO rebirthSO;
    public int ringOfDoom;

    [Header("UI Objects")]
    public Text levelTxt;
    public Image expBar;
    public Text hpTxt;
    public Image hpBar;
    public Image barrierBar;
    public Text skillCountText;
    public GameObject damageEffect;

    ThirdPersonController controller;
    GameMgr gameMgr;

    RangeSensor pickupSensor; //필드아이템 감지센서
    
    //임시용
    ItemFunction itemFunction;
    Inventory inventory;

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
        pickupSensor = GetComponentInChildren<RangeSensor>();
        inventory = GetComponent<Inventory>();

        //============게임 시작시 스탯 초기화============
        //레벨 관련
        level = 1;
        //maxExp = 100;
        curExp = 0;
        
        //체력 관련
        //maxHp = 200;
        curHp = maxHp;
        //hpRegen = 0.5f;
        hpRegenTimeout = 1.0f;
        //hpGrowth = 10;
        hpRegenTimeoutDelta = hpRegenTimeout;

        //방어 관련
        block = 0;

        //공격 관련
        //attackDamage = 20;
        //basicAttackSpeed = 2;
        attackSpeed = basicAttackSpeed;
        attackSpeedIncreaseRate = 1.0f;
        controller.shotTimeout = 1 / attackSpeed;
        critChance = 0;
        critDmgRate = 2.0f;

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
        expBar.fillAmount = 0;
        hpBar.fillAmount = 1;
        hpTxt.text = curHp.ToString() + "/" + maxHp.ToString();

    }

    void Update()
    {
        HpRegen();
        RefreshHpBar();

        if(skillMaxCount > 1)
        {
            skillCountText.text = skillCount.ToString();
            skillCountText.gameObject.SetActive(true);
        }
    }

    public void GainExp(int value) //경험치 획득 함수
    {
        curExp += value; //경험치 추가
        if(expBar != null)
        {
            expBar.fillAmount = (float)curExp / (float)maxExp; //경험치바의 게이지 변경
        }
        if (curExp >= maxExp) //경험치가 최대 경험치를 넘으면(레벨업)
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
        maxExp += 20; //필요 경험치량 증가
        expBar.fillAmount = (float)curExp / (float)maxExp; //경험치바의 게이지 변경

        IncreaseMaxHp(hpGrowth); //성장체력만큼 체력 증가
        IncreaseAttackDamage(attackDamageGrowth); //성장공격력만큼 공격력 증가
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

    void RefreshHpBar()
    {
        hpTxt.text = curHp.ToString("F0") + "/" + maxHp.ToString();
        hpBar.fillAmount = curHp / (float)maxHp;
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

    public void IncreaseAttackDamage(float val) //공격력 증가
    {
        attackDamage += val;
    }

    public void IncreaseDamageBonus(float val) //공격력 퍼센트 증가
    {
        attackDamageBonusRate += val;
    }

    public void DecreaseDamageBonus(float val) //공격력 퍼센트 감소
    {
        attackDamageBonusRate -= val;
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

    public void IncreasePickUpRange(int val)
    {
        pickupSensor.Sphere.Radius += val;
    }

    public void IncreaseSkillCount(int val)
    {
        skillCount += val;
        skillMaxCount += val;
    }



    //=============데미지 계산================
    public float DamageCalc(float rate) //플레이어가 주는 데미지 계산(배율)
    {
        float dmg = (attackDamage + (attackDamage * attackDamageBonusRate)) * rate;

        if(critChance >= 100) //치명타 확률이 100%를 넘겼을 때 확정 치명타
        {
            dmg = dmg * critDmgRate;
            Heal(vampiricTooth*5); //치명타시 흡혈 아이템 적용
        }
        else if(critChance <= 0) //치명타 확률이 0%일 때
        {
            dmg = dmg;
        }
        else //치명타 확률이 1%~99%일 때
        {
            int rand = Random.Range(1, 101); //1~100 랜덤
            if(rand <= critChance) //치명타가 떴을 때
            {
                dmg = dmg * critDmgRate;
                Heal(vampiricTooth*5); //치명타시 흡혈 아이템 적용
            }
            else
            {
                dmg = dmg;
            }
        }

        Heal(leechingSeed);
        return dmg;
    }

    public void PlayerTakeDamage(float dmg) //플레이어가 받는 데미지
    {
        if(curHp > 0) //살아 있는 동안에만
        {
            StartCoroutine(DamageEffectOn());
        }

        if (curHp - dmg <= 0 && rebirthSO != null) //부활 아이템 보유 중 치명적인 피해를 입으면
        {
            if(rebirthSO.quantity > 0) //부활 아이템이 남아 있다면
            {
                UseRebirth(); //부활 사용
            }
            else
            {
                curHp -= dmg;
            }
        }
        else
        {
            curHp -= dmg;
        }
    }

    IEnumerator DamageEffectOn() //데미지 이펙트 키는 함수
    {
        damageEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f); //0.1초 동안만
        damageEffect.SetActive(false);
    }

    public void Heal(float val) //체력 회복 함수
    {
        curHp += val;
        if(curHp > maxHp)
        {
            curHp = maxHp;
        }
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

    public void SprintMushroom(int val, int quantity)
    {
        itemFunction.ExeSprintMushroom(val, quantity);
    }

    public void FireRoad(GameObject pref, int quantity)
    {
        itemFunction.GenFireRoad(pref, quantity);
    }

    public void SetLeechingSeed(int val)
    {
        leechingSeed = val;
    }

    public void SetVampiricTooth(int val)
    {
        vampiricTooth = val;
    }

    public void FocusOn(float val, int quantity)
    {
        itemFunction.FocusOn(val, quantity);
    }

    public void SetLuckyShot(float val, int quantity)
    {
        if(quantity == 1)
        {
            luckyShotRate = val;
        }
        else if(quantity >= 2)
        {
            //곱연산
            luckyShotRate = 1 - 1 / (1 + (val * quantity));
        }
    }

    public void SetRebirth(ItemSO item)
    {
        rebirthSO = item; //부활 아이템SO 가져오기
    }

    public void UseRebirth()
    {
        inventory.RemoveItem(rebirthSO); //인벤토리에서 부활 아이템 하나 제거
        gameMgr.RefreshInventory(); //인벤토리 새로고침
        curHp = maxHp; //체력 최대로 회복
    }

    public void SetRingOfDoom(int quantity)
    {
        ringOfDoom = quantity;
    }
}