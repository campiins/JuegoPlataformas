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

    [Header("Audio")]

    [SerializeField] private AudioClip deathSound;

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
        Bat bat = this.gameObject.GetComponent<Bat>();
        Slime slime = this.gameObject.GetComponent<Slime>();
        Skeleton skeleton = this.gameObject.GetComponent<Skeleton>();
        Wizard wizard = this.gameObject.GetComponent<Wizard>();

        if (bat != null)
        {
            Animator batAnimator = bat.GetComponent<Animator>();
            if ( batAnimator != null && isDead)
            {
                batAnimator.SetTrigger("explosion");
                audioSource.PlayOneShot(deathSound, 1f);
            }
            else
            {
                Debug.LogWarning("Animator not found on Bat enemy");
            }
        }
        else if (skeleton != null)
        {
            Animator skeletonAnimator = skeleton.GetComponent<Animator>();
            if (skeletonAnimator != null && isDead)
            {
                skeletonAnimator.SetBool("walking", false);
                skeletonAnimator.SetBool("attacking", false);
                skeletonAnimator.SetBool("death", true);
                audioSource.PlayOneShot(deathSound, 0.25f);
            }
            else
            {
                Debug.LogWarning("Animator not found on Skeleton enemy");
            }
        }
        else if (slime != null)
        {
            Animator slimeAnimator = slime.GetComponent<Animator>();
            if (slimeAnimator != null && isDead)
            {
                slimeAnimator.SetBool("attacking", false);
                slimeAnimator.SetBool("death", true);
                audioSource.PlayOneShot(deathSound, 0.5f);
            }
            else 
            {
                Debug.LogWarning("Animator not found on Slime enemy");
            }
        }
        else if (wizard != null)
        {
            Animator wizardAnimator = wizard.GetComponent<Animator>();
            if (wizardAnimator != null && isDead)
            {
                wizardAnimator.SetBool("death", true);
                audioSource.PlayOneShot(deathSound, 1f);
            }
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
