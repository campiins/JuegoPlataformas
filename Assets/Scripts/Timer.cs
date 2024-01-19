using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject gameCompleteScreen;

    int minutes;
    int seconds;

    void Update()
    {
        if (gameOverScreen.activeInHierarchy == false && gameCompleteScreen.activeInHierarchy == false)
        {
            PublicTimer.timePlayed += Time.deltaTime;
            minutes = Mathf.FloorToInt(PublicTimer.timePlayed / 60);
            seconds = Mathf.FloorToInt(PublicTimer.timePlayed % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }
}
