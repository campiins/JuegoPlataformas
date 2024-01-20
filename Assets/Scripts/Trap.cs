using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] AudioClip spikeDeathSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealthSystem = other.GetComponent<PlayerHealthSystem>();
            float damageToPlayer = playerHealthSystem.maxHealth;
            playerHealthSystem.TakeDamage(damageToPlayer);
            audioSource.PlayOneShot(spikeDeathSound, 0.5f);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealthSystem enemyHealthSystem = other.GetComponent<EnemyHealthSystem>();
            float damageToEnemy = enemyHealthSystem.maxHealth;
            enemyHealthSystem.TakeDamage(damageToEnemy);
            audioSource.PlayOneShot(spikeDeathSound, 0.5f);
        }
    }
}
