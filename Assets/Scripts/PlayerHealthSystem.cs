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
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isDead = false;

    [SerializeField] AudioClip hitSound;

    public event Action OnPlayerDeath;

    private Rigidbody2D rb;
    private Animator anim;
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
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        damageFlash = GetComponent<DamageFlash>();

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
        GetComponent<AudioSource>().PlayOneShot(hitSound, 0.33f);

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
