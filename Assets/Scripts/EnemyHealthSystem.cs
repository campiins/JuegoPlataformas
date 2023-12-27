using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    public int lives;
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isDead = false;

    public event Action OnEnemyDeath;

    private AudioSource audioSource;
    private DamageFlash damageFlash;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        PlayerUIManager playerUIManager = FindObjectOfType<PlayerUIManager>();
        if (playerUIManager != null) playerUIManager.UpdateScore();

        this.gameObject.SetActive(false);
    }
}
