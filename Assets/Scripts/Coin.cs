using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;
    [SerializeField] private AudioSource sound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            sound.Play();
            Destroy(gameObject);
            CoinCounter.instance.IncreaseCoins(value);
        }
    }
}
