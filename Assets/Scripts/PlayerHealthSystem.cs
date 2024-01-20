using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthSystem : MonoBehaviour
{
    private static PlayerHealthSystem instance;

    public static PlayerHealthSystem Instance { get { return instance; } }

    public int lives;
    public float maxHealth;
    [NonSerialized] public float currentHealth;
    [NonSerialized] public bool isDead = false;

    [Header("Audio")]

    [SerializeField] AudioClip hitSound;

    public event Action OnPlayerDeath;

    private Rigidbody2D rb;
    private PlayerController controller;
    private Animator anim;
    private AudioSource audioSource;
    private DamageFlash damageFlash;

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

        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        damageFlash = GetComponent<DamageFlash>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (!PlayerPrefs.HasKey("PlayerLives")) // Si no existe la key 'PlayerLives'
        {
            PlayerPrefs.SetInt("PlayerLives", lives);
        }
        else if (SceneManager.GetActiveScene().buildIndex > 1) // Si existe y es el nivel 2 o más
        {
            lives = PlayerPrefs.GetInt("PlayerLives");
        }
        else // Si es una escena anterior al nivel 2
        {
            PlayerPrefs.SetInt("PlayerLives", lives);
        }

        // Actualizar texto de vidas
        GameManager.Instance.UpdatePlayerLives(lives);
    }

    public void TakeDamage(float damage)
    {
        anim.SetTrigger("hurt");
        audioSource.PlayOneShot(hitSound, 0.33f);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0)
        {
            if (lives > 0)
            {
                lives--;
                PlayerPrefs.SetInt("PlayerLives", lives);
                if (lives <= 0)
                {
                    Die();
                }
                else
                {
                    PlayerController.Instance.Respawn();
                }
            }
        }

        if (damageFlash != null)
        {
            damageFlash.CallDamageFlash();
        }
    }
    
    public void TakeDamage(float damage, float knockbackForce, Vector2 knockbackDirection)
    {
        anim.SetTrigger("hurt");
        audioSource.PlayOneShot(hitSound, 0.33f);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Knockback 
        Knockback(knockbackForce, knockbackDirection);

        if (currentHealth <= 0)
        {
            if (lives > 0)
            {
                lives--;
                PlayerPrefs.SetInt("PlayerLives", lives);
                if (lives <= 0)
                {
                    Die();
                }
                else
                {
                    PlayerController.Instance.Respawn();
                }
            }
        }

        if (damageFlash != null)
        {
            damageFlash.CallDamageFlash();
        }
    }

    private void Knockback(float knockbackForce, Vector2 knockbackDirection)
    {
        controller.isKnockbacked = true;
        Vector2 previousVelocity = rb.velocity;
        rb.AddForceAtPosition(knockbackDirection * knockbackForce, transform.position);
        StartCoroutine(StopKnockback(previousVelocity));
    }

    private IEnumerator StopKnockback(Vector2 velocity)
    {
        yield return new WaitForSeconds(controller.knockbackedTime);
        controller.isKnockbacked = false;
        rb.velocity = velocity;
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        isDead = true;
        anim.SetBool("death", true);
        rb.velocity = Vector2.zero;
    }

    public void OnDeathAnimationFinished()
    {
        if (PlayerController.Instance.isGrounded) 
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
