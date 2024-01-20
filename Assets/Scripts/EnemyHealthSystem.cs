using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    public int lives;
    public float maxHealth;
    [NonSerialized] public float currentHealth;
    [NonSerialized] public bool isDead = false;

    [Header("Knockback")]
    [SerializeField] private float knockbackedTime = 0.5f;
    [NonSerialized] public bool isKnockbacked = false;

    public event Action OnEnemyDeath;

    private Rigidbody2D rb;
    private DamageFlash damageFlash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        damageFlash = GetComponent<DamageFlash>();

        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth <= 0)
        {
            if (lives > 0)
            {
                lives--;
                if (lives <= 0)
                {
                    isDead = true;
                    ActivateDeathAnimation();
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
        isKnockbacked = true;
        Vector2 previousVelocity = rb.velocity;
        rb.AddForceAtPosition(knockbackDirection * knockbackForce, transform.position);
        StartCoroutine(StopKnockback(previousVelocity));
    }

    private IEnumerator StopKnockback(Vector2 velocity)
    {
        yield return new WaitForSeconds(knockbackedTime);
        isKnockbacked = false;
        rb.velocity = velocity;
    }

    private void ActivateDeathAnimation()
    {
        IEnemyDeath enemyDeath = GetComponent<IEnemyDeath>();
        if (enemyDeath != null && isDead)
        {
            enemyDeath.PlayDeathAnimation();
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        OnEnemyDeath?.Invoke();

        GameManager.Instance.enemiesKilled++;
        PlayerPrefs.SetInt("enemiesKilled", GameManager.Instance.enemiesKilled);
        // Actualizar texto score
        GameManager.Instance.UpdateScore();

        this.gameObject.SetActive(false);
    }
}
