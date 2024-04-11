using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    Animator animator;
    Transform playerTr;
    GameMgr gameMgr;

    public Collider attackHitBox;
    public ParticleSystem attackEffect;
    public AudioSource attackSound;
    public Text dmgTextPref;
    public Canvas monCanvas;
    public GameObject HpUI;
    public Image HpBar;

    public Monster mon;

    public bool damageApplied = false;
    bool levelUpFlag = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerTr = GameObject.Find("PlayerTransform").GetComponent<Transform>();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        monCanvas = GetComponentInChildren<Canvas>();

        mon.Init();
    }

    private void OnEnable()
    {
        mon.Respawn();
    }

    void Update()
    {
        mon.Action(playerTr, animator, gameMgr);
        
        if((int)gameMgr.playTime % gameMgr.monsterLevelUpTime == 0 && levelUpFlag == false) //몬스터 레벨업 주기마다 스탯 증가
        {
            mon.monMaxHP += mon.monBasicMaxHP * 0.3f;
            mon.monCurHP += mon.monBasicMaxHP * 0.3f;

            mon.attackPower += mon.basicAttackPower * 0.2f;
            levelUpFlag = true;
        }
        else if ((int)gameMgr.playTime % gameMgr.monsterLevelUpTime >= 1)
        {
            levelUpFlag = false;
        }

        if(mon.monStat == MonsterStat.Death) //죽으면 hpUI끄기
        {
            HpUI.SetActive(false);
        }

        //UI가 항상 일정 크기로 보이게 해주기
        Vector3 monPos = transform.position;
        monPos.y = 0;
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = 0;
        float distance = Vector3.Distance(monPos, camPos);
        if(monCanvas != null)
        {
            monCanvas.transform.localScale = Vector3.one * (distance / 10f); // 10은 임의의 값으로 조절 가능
        }

        //중복 피해 방지 플래그가 켜지면
        if(damageApplied)
        {
            StartCoroutine(DamageApplyCount());
        }
    }

    //몬스터가 피해를 받는 함수
    public void Damaged(float val)
    {
        HpUI.SetActive(true);
        //Text clone = Instantiate(dmgTextPref, dmgCanvas.transform);
        //Destroy(clone, 2.0f);
        mon.monCurHP -= val;
        HpBar.fillAmount = mon.monCurHP / mon.monMaxHP;
    }

    public void ActivateAttackHitbox() //공격 모션 시작시 호출될 함수
    {
        if(attackHitBox != null)
        {
            attackHitBox.enabled = true; //공격 판정 범위 활성화
        }
        if (attackEffect != null)
        {
            attackEffect.Play(); //공격 이펙트 재생
        }
        if (attackSound != null)
        {
            attackSound.Play(); //공격 사운드 재생
        }
    }

    public void DeactivateAttackHitbox() //공격 모션이 끝날 때 호출될 함수
    {
        if(attackHitBox != null)
        {
            attackHitBox.enabled = false; //공격 판정 범위 비활성화
        }
    }

    //일정 시간 뒤에 중복 피해 방지 플래그 끄는 코루틴
    public IEnumerator DamageApplyCount()
    {
        yield return new WaitForSeconds(1.0f);
        damageApplied = false;
    }
}
