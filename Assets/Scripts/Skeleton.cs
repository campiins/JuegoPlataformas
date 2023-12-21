using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
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

    [Header("Ground Check")]

    [SerializeField] private float raycastDistance;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Audio")]

    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip attackSound;

    private bool isChasing = false;
    private float distanceToPlayer;

    private Vector3 currentDestination;
    private int currentIndex = 0;

    private Animator anim;
    private AudioSource audioSource;
    private PlayerHealthSystem playerHealthSystem;
    private EnemyHealthSystem enemyHealthSystem;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        enemyHealthSystem = GetComponent<EnemyHealthSystem>();
        playerHealthSystem = PlayerHealthSystem.Instance;

        currentDestination = waypoints[currentIndex].position;
        StartCoroutine(Patrol());
    }

    void Update()
    {
        // Lanzar un rayo hacia abajo para detectar el suelo
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, Vector2.down, raycastDistance, whatIsGround);
        Debug.DrawRay(attackPoint.position, Vector2.down * raycastDistance, Color.red);
        
        if (isChasing && !enemyHealthSystem.isDead)
        {
            distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            ChasePlayer();

            if (hit.collider == null)
            {
                anim.SetBool("walking", false);
            }
            else if (distanceToPlayer > attackDistance)
            {
                anim.SetBool("walking", true);
            }
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
        anim.SetBool("walking", true);
        while (true)
        {
            while (transform.position != currentDestination)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
                yield return null;
            }
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
        Transform playerTransform = PlayerController.Instance.gameObject.transform;

        if (playerTransform != null && !playerHealthSystem.isDead)
        {
            if (distanceToPlayer <= attackDistance)
            {
                if (canAttack)
                {
                    anim.SetBool("attacking", true);
                }

                // No es necesario moverse mientras ataca
                return;
            }

            // Limitar posición X y obtener la posición del jugador ignorando los ejes Y y Z
            float targetX = Mathf.Clamp(playerTransform.position.x, waypoints[0].position.x, waypoints[waypoints.Length - 1].position.x);
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

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
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed + 1) * Time.deltaTime);
        }
        else if (playerHealthSystem.isDead) // Si el jugador está muerto
        {
            if (anim.GetBool("attacking") == true)
            {
                anim.SetBool("attacking", false);
            }
            BackToPatrol();
        }
    }

    // Se ejecuta desde evento de animación.
    private void Attack()
    {
        if (canAttack)
        {
            audioSource.PlayOneShot(attackSound, 0.8f);
            // Lanzar Trigger
            Collider2D[] collidersHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsDamageable);
            foreach (Collider2D col in collidersHit)
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    playerHealthSystem.TakeDamage(attackDamage);
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
        anim.SetBool("attacking", false);
        anim.SetBool("walking", false);
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
    public void PlayFootstepSound()
    {
        audioSource.PlayOneShot(footstepSound, 0.05f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerDetection"))
        {
            // Player detectado
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
}
