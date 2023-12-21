using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform platform;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [Range(1,4)]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitime = 1f;

    private int direction = 1;
    private bool isMoving = true;

    void Start()
    {
        platform.position = startPoint.position;
    }

    void Update()
    {
        Vector2 target = currentMovementTarget();

        if (isMoving)
        {
            platform.position = Vector2.MoveTowards(platform.position, target, speed * Time.deltaTime);

            float distance = (target - (Vector2) platform.position).magnitude;
            if (distance <= 0.1f)
            {
                direction *= -1;
                StartCoroutine(Wait(waitime));
            }
        }
    }

    Vector2 currentMovementTarget()
    {
        if (direction == 1) 
        {
            return endPoint.position;
        }
        else
        {
            return startPoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        // Debug visualization
        if (platform != null && startPoint != null && endPoint != null)
        {
            Gizmos.DrawLine(platform.position, startPoint.position);
            Gizmos.DrawLine(platform.position, endPoint.position);
        }
    }

    IEnumerator Wait(float seconds)
    {
        isMoving = false;
        yield return new WaitForSeconds(seconds);
        isMoving = true;
    }
}
