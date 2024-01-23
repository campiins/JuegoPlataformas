using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private static LevelChanger instance;

    public static LevelChanger Instance { get { return instance; } }

    private int levelToLoad;
    
    private Animator anim;

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

        anim = GetComponent<Animator>();
    }

    public void FadeToNextLevel()
    {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FadeToPreviousLevel()
    {
        PublicTimer.timePlayed = 0;
        FadeToLevel(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void FadeToMenu()
    {
        PublicTimer.timePlayed = 0;
        FadeToLevel(0);
    }

    public void FadeToCredits()
    {
        PublicTimer.timePlayed = 0;
        FadeToLevel(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void OpenFirstLevel()
    {
        PublicTimer.timePlayed = 0;
        SceneManager.LoadScene(1);
    }

    public void FadeToLevel(int buildIndex)
    {
        levelToLoad = buildIndex;
        anim.SetTrigger("FadeOut");
    }

    // Se ejecuta desde evento de animación.
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void RestartLevel()
    {
        PublicTimer.timePlayed = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
