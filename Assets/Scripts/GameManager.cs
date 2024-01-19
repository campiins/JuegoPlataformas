using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    [HideInInspector] public int enemiesKilled = 0;
    [HideInInspector] public bool gameCompleted = false;

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
                enemiesKilled = PlayerPrefs.GetInt("enemiesKilled");
                // Actualizar texto score
                UpdateScore();
            }
            else
            {
                PlayerPrefs.SetInt("enemiesKilled", 0);
            }
        }
        else
        {
            PlayerPrefs.SetInt("enemiesKilled", enemiesKilled);
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

        //Debug.Log("Press 'ESC' to return to menu.");
    }

    void Update()
    {
        float playerHealth = playerHealthSystem.currentHealth;
        // Actualizar barra de vida
        if (healthBar.value != playerHealth)
        {
            healthBar.value = playerHealth;
        }

        // Volver al menú principal
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    LevelChanger.Instance.FadeToMenu();
        //}
    }

    public void GameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.Setup(enemiesKilled);
        }
    }

    public void GameCompleted()
    {
        gameCompleted = true;
        // Detener animación de correr
        PlayerController.Instance.GetComponent<Animator>().SetBool("running", false);
        if (gameCompletedScreen != null)
        {
            gameCompletedScreen.Setup(enemiesKilled);
        }
    }

    public void UpdateScore()
    {
        scoreText.text = "Kills: " + enemiesKilled.ToString();
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
