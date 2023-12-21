using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private float damage;

    [SerializeField] AudioClip spikeDeathSound;

    private PlayerHealthSystem playerHealthSystem;
    private AudioSource audioSource;

    void Start()
    {
        playerHealthSystem = PlayerHealthSystem.Instance;
        audioSource = GetComponent<AudioSource>();

        damage = playerHealthSystem.maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealthSystem>().TakeDamage(damage);
            audioSource.PlayOneShot(spikeDeathSound, 0.5f);
        }
    }
}
