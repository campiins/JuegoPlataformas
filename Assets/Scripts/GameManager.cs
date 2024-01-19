using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    public AudioManager audioManager;
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private GameCompletedScreen gameCompletedScreen;
    
    [HideInInspector] public int enemiesKilled = 0;
    [HideInInspector] public bool gameCompleted = false;

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
                PlayerUIManager playerUIManager = FindObjectOfType<PlayerUIManager>();
                if (playerUIManager != null) playerUIManager.UpdateScore();
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

        //Debug.Log("Press 'ESC' to return to menu.");
    }

    void Update()
    {
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
        PlayerController.Instance.GetComponent<Animator>().SetBool("running", false);
        if (gameCompletedScreen != null)
        {
            gameCompletedScreen.Setup(enemiesKilled);
        }
    }
}
