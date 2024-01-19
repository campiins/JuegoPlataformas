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
    private bool isFacingRight = true;

    [Header("Movement")]

    [SerializeField] private float movementSpeed;

    [Header("Jump")]

    [SerializeField] private float jumpPower = 18f;
    [SerializeField] private float doubleJumpPower = 20f;
    [SerializeField] private float coyoteTime = 0.1f;
    private bool canDoubleJump = false;
    private bool inCoyoteTime = false;
    private float coyoteTimeCounter;

    [Header("Dash")]

    [SerializeField] private float dashPower;
    [SerializeField] private float dashingTime = 0.2f;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingCooldown = 0.5f;

    [Header("Wall Jump")]

    [SerializeField] [Range(0.1f, 2f)] private float wallSlidingSpeed = 1f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(10f, 18f);
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.4f, 1.5f);
    [SerializeField] private LayerMask whatIsWall;
    private bool isWalled = false;
    private bool isWallSliding = false;
    private bool isWallJumping = false;
    private bool canWallJump = false;
    private float wallJumpingDirection;

    [Header("Attack")]

    [SerializeField] private float attackDamage;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask whatIsDamageable;

    [Header("Ground Check")]

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckSize = 0.15f;
    [SerializeField] private LayerMask whatIsGround;
    [HideInInspector] public bool isGrounded;

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
        inputH = Input.GetAxisRaw("Horizontal");

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

            if (isWalled && rb.velocity.x == 0 || isGrounded)
            {
                StopWallJumping();
            }

            anim.SetFloat("yVelocity", rb.velocity.y);

            LaunchAttack();

            Jump();

            WallSlide();
            WallJump();

            if (!isWallSliding && !isWallJumping)
            {
                Movement();
                Flip();
            }

            Dash();

            anim.SetBool("wallSliding", isWallSliding);
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
    }

    private void Movement()
    {
        rb.velocity = new Vector2(inputH * movementSpeed, rb.velocity.y);
        if (inputH != 0) // Hay movimiento.
        {
            anim.SetBool("running", true);
        }
        else // No hay movimiento (inputH = 0)
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

        if (Input.GetButtonDown("Jump") && (isGrounded || inCoyoteTime || canDoubleJump) && !isWallSliding)
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

        // Disminuye la velocidad vertical si se deja de presionar el botón de salto
        if (Input.GetButtonUp("Jump"))
        {
            anim.SetBool("jump", false);

            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.75f);
        }
    }
    private void WallSlide()
    {
        if (isWalled && !isGrounded && (inputH != 0f))
        {
            isWallSliding = true;
            if (rb.velocity.y < -wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            wallJumpingDirection = -transform.forward.z;
            canWallJump = true;

            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && isWalled && canWallJump)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            canWallJump = false;

            if (transform.forward.z != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 currentRotation = transform.eulerAngles;
                currentRotation.y = isFacingRight ? 0f : -180f;
                transform.eulerAngles = currentRotation;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
        canWallJump = false;
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckSize, whatIsGround); ;

        anim.SetBool("jump", !isGrounded);

        if (isGrounded)
        {
            inCoyoteTime = true;
            coyoteTimeCounter = 0;
        }
    }

    private void WallCheck()
    {
        isWalled = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, whatIsWall);

        if (isWalled)
        {
            canDoubleJump = false;
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

    private void Flip()
    {
        if ( (isFacingRight && inputH < 0f) || (!isFacingRight && inputH > 0f) )
        {
            isFacingRight = !isFacingRight;
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = isFacingRight ? 0f : -180f;
            transform.eulerAngles = currentRotation;
        }
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        PlayerHealthSystem.Instance.isDead = false;
        PlayerHealthSystem.Instance.currentHealth = PlayerHealthSystem.Instance.maxHealth;

        GameManager.Instance.UpdatePlayerLives(PlayerHealthSystem.Instance.lives);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
