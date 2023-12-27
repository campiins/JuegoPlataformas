using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour, IEnemyDeath
{
    [Header("Attack")]

    [SerializeField] private GameObject fireball; // Prefab.
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackDamage;

    [Header("Audio")]

    [SerializeField] private AudioClip throwSpellSound;
    [SerializeField] private AudioClip deathSound;

    private Animator anim;
    private AudioSource audioSource;
    private PlayerHealthSystem playerHealthSystem;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealthSystem = PlayerHealthSystem.Instance;

        StartCoroutine(AttackCoroutine());
    }

    void Update()
    {
        if (playerHealthSystem != null && playerHealthSystem.isDead) StopAllCoroutines();
    }

    // Se ejecuta desde evento de animación. 'Has Exit Time' determina cuántas bolas lanza cada vez.
    private void ThrowFireball()
    {
        // Activar audio de hechizo aquí
        audioSource.PlayOneShot(throwSpellSound, 0.33f);
        Instantiate(fireball, spawnPoint.position, transform.rotation);
    }

    IEnumerator AttackCoroutine()
    {
        while (true)
        {
            anim.SetTrigger("attack");
            yield return new WaitForSeconds(attackDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerDetection"))
        {
            if (transform.position.x > other.gameObject.transform.position.x)
            {
                // Player a la izquierda
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (transform.position.x < other.gameObject.transform.position.x)
            {
                // Player a la derecha
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerDetection"))
        {
            if (transform.position.x > other.gameObject.transform.position.x)
            {
                // Player a la izquierda
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (transform.position.x < other.gameObject.transform.position.x)
            {
                // Player a la derecha
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerDetection"))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void PlayDeathAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("death", true);
            audioSource.PlayOneShot(deathSound, 1f);
        }
    }
}

