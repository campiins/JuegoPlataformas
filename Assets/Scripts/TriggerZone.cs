using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private bool oneShot = false;
    private bool alreadyEntered = false;
    private bool alreadyExited = false;

    [SerializeField] private string collisionTag;

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyEntered) return;

        if (!string.IsNullOrEmpty(collisionTag) && !other.CompareTag(collisionTag))
            return;

        onTriggerEnter?.Invoke();

        if (oneShot) alreadyEntered = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (alreadyExited) return;

        if (!string.IsNullOrEmpty(collisionTag) && !other.CompareTag(collisionTag))
            return;

        onTriggerExit?.Invoke();

        if (oneShot) alreadyExited = true;
    }
}
