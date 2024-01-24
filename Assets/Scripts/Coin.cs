using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private AudioSource sound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            sound.Play();
            //CoinCounter.instance.IncreaseCoins(value);
            GameManager.Instance.score += value;
            GameManager.Instance.UpdateScoreText();
            Destroy(gameObject);
        }
    }
}
