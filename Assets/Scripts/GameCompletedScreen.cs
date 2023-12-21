using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCompletedScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        scoreText.text = score.ToString() + " KILLS";
    }
}
