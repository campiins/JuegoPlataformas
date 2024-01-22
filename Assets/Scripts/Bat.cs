using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, IEnemyDeath
{
    [Header("Movement")]

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed;

    [Header("Attack")]

    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask whatIsDamageable;
    private bool canAttack = true;
    private float timeSinceLastAttack = 0f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 150f;

    [Header("Audio")]

    [SerializeField] private AudioClip wingSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    private bool isChasing = false;

    private Vector3 currentDestination;
    private int currentIndex = 0;

    private Animator anim;
    private AudioSource audioSource;
    private EnemyHealthSystem healthSystem;
    private PlayerHealthSystem playerHealthSystem;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthSystem = GetComponent<EnemyHealthSystem>();
        playerHealthSystem = PlayerHealthSystem.Instance;

        currentDestination = waypoints[currentIndex].position;
        if (timeSinceLastAttack < attackCooldown) timeSinceLastAttack = attackCooldown;
        StartCoroutine(Patrol());
    }

    void Update()
    {
        if (isChasing && !healthSystem.isKnockbacked && !healthSystem.isDead)
        {
            ChasePlayer();
        }

        if (timeSinceLastAttack < attackCooldown)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
        else if (!canAttack)
        {
            canAttack = true;
        }
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            while (transform.position != currentDestination)
            {
                // Mover hacia el punto de patrulla actual mientras no haya llegado
                transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
                yield return null;
            }
            // Si ha llegado, pasar al siguiente punto de patrulla
            DefineNewDestination();
        }
    }

    private void DefineNewDestination()
    {
        currentIndex++;
        if (currentIndex >= waypoints.Length)
        {
            currentIndex = 0;
        }
        currentDestination = waypoints[currentIndex].position;
        FocusDestination();
    }

    private void FocusDestination()
    {
        if (currentDestination.x > transform.position.x)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void ChasePlayer()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerTransform != null && !playerHealthSystem.isDead)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackDistance)
            {
                if (canAttack && !healthSystem.isKnockbacked)
                {
                    anim.SetBool("attack", true);
                }

                // No es necesario moverse mientras ataca
                return;
            }

            // Rotar en la dirección del Player
            if (playerTransform.position.x > transform.position.x)
            {
                transform.eulerAngles = Vector3.zero;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            // Mover hacia la posición del Player
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        }
        else if (playerHealthSystem.isDead) // Si el jugador está muerto
        {
            BackToPatrol();
        }
    }

    private void BackToPatrol()
    {
        // Ha dejado de detectar al Player
        isChasing = false;

        StopAllCoroutines();

        // Volver al primer punto de patrulla y seguir patrullando
        currentIndex = 0;
        FocusDestination();
        if (this.gameObject.activeSelf) { StartCoroutine(Patrol()); }
    }

    // Se ejecuta desde evento de animación.
    private void Attack()
    {
        if (canAttack)
        {
            audioSource.PlayOneShot(attackSound, 1f);
            // Lanzar Trigger
            Collider2D[] collidersHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsDamageable);
            foreach (Collider2D col in collidersHit)
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    Vector2 knockbackDirection = transform.right + (transform.up * 0.25f);
                    playerHealthSystem.TakeDamage(attackDamage, knockbackForce, knockbackDirection);
                }
            }
            // Iniciar cooldown
            timeSinceLastAttack = 0f;
        }
    }

    // Se ejecuta desde evento de animación.
    private void AttackAnimationFinished()
    {
        canAttack = false;
        anim.SetBool("attack", false);
    }

    // Se ejecuta desde evento de animación.
    public void PlayWingSound()
    {
        audioSource.PlayOneShot(wingSound, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerDetection"))
        {
            // Player detectado
            anim.SetTrigger("surprise");
            isChasing = true;

            // Dejar de patrullar
            StopAllCoroutines();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerDetection"))
        {
            // Ha dejado de detectar al Player
            BackToPatrol();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void PlayDeathAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("explosion");
            audioSource.PlayOneShot(deathSound, 1f);
        }
    }
}

