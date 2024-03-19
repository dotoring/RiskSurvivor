using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    Animator animator;
    GameObject player;
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

    void Start()
    {
        animator = GetComponent<Animator>();
        //player = GameObject.Find("Player");
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
        if(mon.monStat == MonsterStat.Death)
        {
            HpUI.SetActive(false);
        }
        Vector3 monPos = transform.position;
        monPos.y = 0;
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = 0;
        float distance = Vector3.Distance(monPos, camPos);
        monCanvas.transform.localScale = Vector3.one * (distance / 10f); // 10은 임의의 값으로 조절 가능
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

    private void OnTriggerEnter(Collider other) //공격 판정 범위에 콜리더가 들어왔을 때
    {
        if(other.tag == "Player") //플레이어면 데미지 주기
        {
            PlayerValue.Instance.PlayerTakeDamage(mon.attackPower);
        }
    }
}
