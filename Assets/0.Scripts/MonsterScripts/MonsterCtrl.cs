using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    Animator animator;
    GameObject player;
    Transform playerTr;
    GameMgr gameMgr;

    public Collider attackHitBox;
    public ParticleSystem attackEffect;
    public AudioSource attackSound;

    public Monster mon;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        playerTr = player.GetComponent<Transform>();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();

        mon.Init();
    }

    private void OnEnable()
    {
        mon.Respawn();
    }

    void Update()
    {
        mon.Action(playerTr, animator, gameMgr);
    }

    //몬스터가 피해를 받는 함수
    public void Damaged(int val)
    {
        mon.monCurHP -= val;
    }

    public void ActivateAttackHitbox() //공격 모션 시작시 호출될 함수
    {
        attackHitBox.enabled = true; //공격 판정 범위 활성화
        attackEffect.Play();
        attackSound.Play();
    }

    public void DeactivateAttackHitbox() //공격 모션이 끝날 때 호출될 함수
    {
        attackHitBox.enabled = false; //공격 판정 범위 비활성화
    }

    private void OnTriggerEnter(Collider other) //공격 판정 범위에 콜리더가 들어왔을 때
    {
        if(other.tag == "Player") //플레이어면 데미지 주기
        {
            PlayerValue.Instance.PlayerTakeDamage(mon.attackPower);
        }
    }
}
