using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private GameObject ironDoor;
    private bool activated = false;
    private bool isPlayerInSwitch = false;

    private Animator anim;
    private AudioSource audioSource;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayerInSwitch && !activated)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                activated = true;
                audioSource.Play();
                anim.SetBool("active", true);
                OpenIronDoor();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInSwitch = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
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
