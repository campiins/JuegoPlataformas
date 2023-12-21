using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [SerializeField] private Key requiredKey;
    [SerializeField] private bool isGameCompletedDoor = false;

    private bool isPlayerInDoor = false;

    [Header("UI")]

    [SerializeField] private Canvas doorCanvas;
    private Image doorTxtBackground;
    private TMP_Text doorTxt;
    
    private InventoryManager playerInventory;

    void Start()
    {
        playerInventory = InventoryManager.Instance;

        doorTxtBackground = doorCanvas.GetComponentInChildren<Image>();
        doorTxt = doorTxtBackground.gameObject.GetComponentInChildren<TMP_Text>();
        doorTxtBackground.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInDoor)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerInventory.HasItem(requiredKey.gameObject))
                {
                    playerInventory.RemoveItem(requiredKey.gameObject);
                    OpenDoor();
                }
                else
                {
                    GameManager.Instance.audioManager.PlayDoorLockedSound();
                    if (doorTxtBackground != null && doorTxt != null)
                    {
                        doorTxtBackground.gameObject.SetActive(true);
                        doorTxt.text = "You don't have the <color=yellow>key</color> to open the door.";
                    }
                    Debug.Log("You don't have the key to open the door.");
                }
            }
        }
    }

    private void OpenDoor()
    {
        if (!isGameCompletedDoor)
        {
            GameManager.Instance.audioManager.PlayDoorOpenSound();
            LevelChanger.Instance.FadeToNextLevel();
        }
        else
        {
            if (doorTxtBackground != null)
            {
                doorTxtBackground.gameObject.SetActive(false);
            }
            GameManager.Instance.audioManager.PlayLevelCompletedSound();
            GameManager.Instance.GameCompleted();
            Debug.Log("WELL DONE! You completed the game.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInDoor = true;
            if (doorTxtBackground != null && doorTxt != null)
            {
                doorTxtBackground.gameObject.SetActive(true);
                doorTxt.text = "Press 'E' to open the door.";
            }
            Debug.Log("Press 'E' to open the door.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (doorTxtBackground != null)
            {
                doorTxtBackground.gameObject.SetActive(false);
            }
            isPlayerInDoor = false;
        }
    }
}

