using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lever : MonoBehaviour
{
    [SerializeField] private GameObject ironDoor;
    private bool activated = false;
    private bool isPlayerInSwitch = false;

    [Header("UI")]

    [SerializeField] private Canvas leverCanvas;
    private Image txtBackground;
    private TMP_Text leverTxt;

    private Animator anim;
    private AudioSource audioSource;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        txtBackground = leverCanvas.GetComponentInChildren<Image>();
        leverTxt = txtBackground.gameObject.GetComponentInChildren<TMP_Text>();
        txtBackground.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }
    }

    private void Activate()
    {
        if (isPlayerInSwitch && !activated)
        {
            activated = true;
            audioSource.Play();
            anim.SetBool("active", true);
            OpenIronDoor();

            if (txtBackground != null)
            {
                txtBackground.gameObject.SetActive(false);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInSwitch = true;
            if (!activated)
            {
                if (txtBackground != null && leverTxt != null)
                {
                    txtBackground.gameObject.SetActive(true);
                    leverTxt.text = "[E]";
                }
                Debug.Log("Press 'E' to activate the lever.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (txtBackground != null)
            {
                txtBackground.gameObject.SetActive(false);
            }
            isPlayerInSwitch = false;
        }
    }

    private void OpenIronDoor()
    {
        Animator doorAnimator = ironDoor.GetComponent<Animator>();
        AudioSource doorAS = ironDoor.GetComponent<AudioSource>();
        if (doorAnimator != null && doorAS != null)
        {
            doorAnimator.SetBool("open", true);
            doorAS.Play();
        }
        else
        {
            Debug.LogWarning("IronDoor animator or audioSource component not found");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, ironDoor.transform.position);
    }
}
