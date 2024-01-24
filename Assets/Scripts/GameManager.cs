using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    public AudioManager audioManager;
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private GameCompletedScreen gameCompletedScreen;

    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text scoreText;

    [NonSerialized] public int score = 0;
    [NonSerialized] public bool gameCompleted = false;

    private PlayerHealthSystem playerHealthSystem;

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

        if (PlayerPrefs.HasKey("enemiesKilled"))
        {
            if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                score = PlayerPrefs.GetInt("enemiesKilled");
                // Actualizar texto score
                UpdateScoreText();
            }
            else
            {
                PlayerPrefs.SetInt("enemiesKilled", 0);
            }
        }
        else
        {
            PlayerPrefs.SetInt("enemiesKilled", score);
        }
    }

    void Start()
    {
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = PlayerController.Instance.gameObject.transform;

        playerHealthSystem = PlayerHealthSystem.Instance;
        healthBar.minValue = 0;
        healthBar.maxValue = playerHealthSystem.maxHealth;

        playerHealthSystem.OnPlayerDeath += HandlePlayerDeath;
    }

    void Update()
    {
        float playerHealth = playerHealthSystem.currentHealth;
        // Actualizar barra de vida
        if (healthBar.value != playerHealth)
        {
            healthBar.value = playerHealth;
        }
    }

    public void GameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.Setup(score);
            StartCoroutine(ShowCredits());
        }
    }

    private IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(5f);
        FindFirstObjectByType<LevelChanger>().FadeToCredits();
    }

    public void GameCompleted()
    {
        gameCompleted = true;
        // Detener animación de correr
        PlayerController.Instance.GetComponent<Animator>().SetBool("running", false);
        if (gameCompletedScreen != null)
        {
            gameCompletedScreen.Setup(score);
        }
    }

    public void UpdateScoreText()
    {
        PlayerPrefs.SetInt("enemiesKilled", score);
        scoreText.text = "Score: " + score.ToString();
    }

    public void UpdatePlayerLives(int lives)
    {
        livesText.text = lives.ToString();
    }

    private void HandlePlayerDeath()
    {
        healthBar.value = healthBar.minValue;
        UpdatePlayerLives(playerHealthSystem.lives);
    }
}
