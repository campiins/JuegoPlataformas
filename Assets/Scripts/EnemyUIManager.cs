using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;

    private EnemyHealthSystem enemyHealthSystem;

    void Start()
    {
        enemyHealthSystem = GetComponent<EnemyHealthSystem>();

        healthBar.minValue = 0;
        healthBar.maxValue = enemyHealthSystem.maxHealth;
        healthText.text = enemyHealthSystem.maxHealth.ToString();

        enemyHealthSystem.OnEnemyDeath += HandleEnemyDeath;
    }

    void Update()
    {
        float health = enemyHealthSystem.currentHealth;
        // Actualizar barra de vida
        if (healthBar.value != health && healthText.text != health.ToString())
        {
            healthBar.value = health;
            healthText.text = health.ToString();
        }

        if (transform.rotation.y != 180)
        {
            healthBar.transform.eulerAngles = Vector3.zero;
            healthText.transform.eulerAngles = Vector3.zero;
        }
    }

    private void HandleEnemyDeath()
    {
        healthBar.value = healthBar.minValue;
        healthText.text = 0.ToString();
    }
}
