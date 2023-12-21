using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject chestItem;
    [SerializeField] private float chestDelay;
    private bool opened = false;
    private bool isPlayerInChest = false;

    private Vector3 offset;

    [Header("UI")]

    [SerializeField] private Canvas doorCanvas;
    private Image doorTxtBackground;
    private TMP_Text doorTxt;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        offset = new Vector3(0.033f, 1f, transform.position.z);

        doorTxtBackground = doorCanvas.GetComponentInChildren<Image>();
        doorTxt = doorTxtBackground.gameObject.GetComponentInChildren<TMP_Text>();
        doorTxtBackground.gameObject.SetActive(false);
    }

    void Update()
    { 
        if (isPlayerInChest && !opened)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                opened = true;
                anim.SetTrigger("open");
                StartCoroutine(GetChestItem());
            }
        }
    }

    IEnumerator GetChestItem()
    {
        yield return new WaitForSeconds(chestDelay);
        Instantiate(chestItem, transform.position + offset, Quaternion.identity);
        GameManager.Instance.audioManager.PlayKeySound();
        InventoryManager.Instance.AddItem(chestItem);
        if (doorTxtBackground != null)
        {
            doorTxtBackground.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInChest = true;
            if (!opened)
            {
                if (doorTxtBackground != null && doorTxt != null)
                {
                    doorTxtBackground.gameObject.SetActive(true);
                    doorTxt.text = "Press 'E' to open the chest.";
                }
                Debug.Log("Press 'E' to open the chest.");
            }
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
            isPlayerInChest = false;
        }
    }
}

