using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    public static PlayerController Instance { get { return instance; } }

    private Rigidbody2D rb;
    private Animator anim;
    private TrailRenderer trailRenderer;

    private float inputH;

    [Header("Movement")]

    [SerializeField] private float movementSpeed;

    [SerializeField] private float jumpPower;
    [SerializeField] private float doubleJumpPower;
    [SerializeField] private float coyoteTime = 0.1f;
    private bool canDoubleJump = false;
    private bool inCoyoteTime = false;
    private float coyoteTimeCounter;

    [SerializeField] private float dashPower;
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;

    [Header("Attack")]

    [SerializeField] private float attackDamage;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask whatIsDamageable;

    [Header("Ground Check")]

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    public bool isGrounded;

    private Vector3 respawnPoint;

    [Header("Audio")]

    [SerializeField] AudioClip footstepSound;
    [SerializeField] AudioClip swordSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip doubleJumpSound;
    [SerializeField] AudioClip dashSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();

        respawnPoint = transform.position;
    }

    void Update()
    {    
        if (!PlayerHealthSystem.Instance.isDead && !GameManager.Instance.gameCompleted)
        {
            if (isDashing)
            {
                if (!isGrounded) anim.SetBool("sliding", false);
                return;
            }

            if (!isGrounded & inCoyoteTime)
            {
                coyoteTimeCounter += Time.deltaTime;
                if (coyoteTimeCounter > coyoteTime) inCoyoteTime = false;
            }

            anim.SetFloat("yVelocity", rb.velocity.y);

            Movement();

            Jump();

            LaunchAttack();

            Dash();
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void Movement()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(inputH * movementSpeed, rb.velocity.y);
        if (inputH != 0) // Hay movimiento.
        {
            anim.SetBool("running", true);
            if (inputH > 0) // Dcha.
            {
                transform.eulerAngles = Vector3.zero;
            }
            else // Izq.
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else // inputH = 0.
        {
            anim.SetBool("running", false);
        }
    }


    private void LaunchAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("attack");
        }
    }

    // Se ejecuta desde evento de animación.
    private void Attack()
    {
        audioSource.PlayOneShot(swordSound, 0.2f);
        // Lanzar Trigger
        Collider2D[] collidersHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsDamageable);
        foreach (Collider2D col in collidersHit)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                EnemyHealthSystem enemyHealthSystem = col.gameObject.GetComponent<EnemyHealthSystem>();
                enemyHealthSystem.TakeDamage(attackDamage);
            }
        }
    }

    private void Jump()
    {
        if (isGrounded && !Input.GetButton("Jump"))
        {
            canDoubleJump = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || inCoyoteTime || canDoubleJump) // Si está en el suelo o tiene doble salto
            {
                // Aplicar velocidad de salto
                rb.velocity = new Vector2(rb.velocity.x, canDoubleJump ? doubleJumpPower : jumpPower);
                // Activar animación de salto
                anim.SetBool("jump", true);

                if (!canDoubleJump) // Si está haciendo el primer salto
                {
                    audioSource.PlayOneShot(jumpSound, 0.1f);
                }
                else // Si está haciendo doble salto
                {
                    anim.SetTrigger("doubleJump");
                    audioSource.PlayOneShot(doubleJumpSound, 0.15f);
                }
                canDoubleJump = !canDoubleJump;
            }
        }
        // Disminuye la velocidad vertical si se deja de presionar el botón de salto
        if (Input.GetButtonUp("Jump"))
        {
            anim.SetBool("jump", false);

            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.75f);
        }
    }

    private void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.15f, whatIsGround);
        if (colliders.Length > 0) isGrounded = true;

        anim.SetBool("jump", !isGrounded);

        if (isGrounded)
        {
            inCoyoteTime = true;
            coyoteTimeCounter = 0;
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && rb.velocity.x != 0f) {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        if (isGrounded) anim.SetBool("sliding", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        audioSource.PlayOneShot(dashSound, 0.15f);
        rb.velocity = new Vector2(inputH * dashPower, 0f);
        if (trailRenderer != null) trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        if (trailRenderer != null) trailRenderer.emitting = false;
        rb.gravityScale = originalGravity;
        anim.SetBool("sliding", false);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    // Se ejecuta desde evento de animación.
    public void PlayFootstepSound()
    {
        audioSource.PlayOneShot(footstepSound, 0.05f);
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        PlayerHealthSystem.Instance.isDead = false;
        PlayerHealthSystem.Instance.currentHealth = PlayerHealthSystem.Instance.maxHealth;

        PlayerUIManager playerUIManager = GetComponent<PlayerUIManager>();
        playerUIManager.UpdateLives(PlayerHealthSystem.Instance.lives);
    }

    public void TriggerDead()
    {
        GameManager.Instance.GameOver();
    }

    // Detectar si el Player está encima de una plataforma
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            this.transform.SetParent(other.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            if (other.gameObject != null)
            {
                this.transform.SetParent(null);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.DrawWireSphere(groundCheck.position, 0.15f);
    }
}
