using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text scoreText;

    private PlayerHealthSystem playerHealthSystem;

    void Start()
    {
        playerHealthSystem = PlayerHealthSystem.Instance;
        healthBar.minValue = 0;
        healthBar.maxValue = playerHealthSystem.maxHealth;

        playerHealthSystem.OnPlayerDeath += HandlePlayerDeath;
    }

    void Update()
    {
        float health = playerHealthSystem.currentHealth;
        // Actualizar barra de vida
        if (healthBar.value != health)
        {
            healthBar.value = health;
        }
    }

    public void UpdateScore()
    {
        scoreText.text = "Kills: " + GameManager.Instance.enemiesKilled.ToString();
    }

    public void UpdateLives(int lives)
    {
        livesText.text = lives.ToString();
    }

    private void HandlePlayerDeath()
    {
        healthBar.value = healthBar.minValue;
        UpdateLives(playerHealthSystem.lives);
    }
}
