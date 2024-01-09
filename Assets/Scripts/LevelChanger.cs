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

    public void FadeToLastLevel()
    {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void FadeToMenu()
    {
        FadeToLevel(0);
    }

    public void FadeToCredits()
    {
        FadeToLevel(3);
    }

    public void OpenFirstLevel()
    {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
