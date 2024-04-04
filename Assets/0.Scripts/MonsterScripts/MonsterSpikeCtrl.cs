using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpikeCtrl : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Animator animator;
    public GameObject warning;
    public Collider collider;
    public Collider triggerCollider;
    public AudioSource effectSound;
    public float warningTime;
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpikeExe());
    }

    IEnumerator SpikeExe()
    {
        yield return new WaitForSeconds(warningTime);
        warning.SetActive(false);
        animator.SetTrigger("Attack");
        particleSystem.Play();
        yield return new WaitForSeconds(0.1f);
        effectSound.Play();
        triggerCollider.enabled = true;
        yield return new WaitForSeconds(2.4f);
        triggerCollider.enabled = false;
        collider.enabled = false;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerValue.Instance.PlayerTakeDamage(damage);
        }
    }
}
