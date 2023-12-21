using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerHealthSystem playerHealthSystem;
    private Animator anim;

    [SerializeField] private float minLaunchForce;
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float damage;
    [SerializeField] private float initialLifetime;
    private float lifetime;
    private bool exploded = false;
    
    [SerializeField] private int maxBounces;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        float launchForce = Random.Range(minLaunchForce, maxLaunchForce);
        rb.AddForce(transform.right * launchForce, ForceMode2D.Impulse);
        lifetime = initialLifetime;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealthSystem = player.GetComponent<PlayerHealthSystem>();
        }
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0 && !exploded)
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        anim.SetTrigger("fadeOut");
    }

    // Se ejecuta desde evento de animación.
    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;
            if (playerHealthSystem != null && !PlayerHealthSystem.Instance.isDead)
            {
                playerHealthSystem.TakeDamage(damage);
            }
            FadeOut();
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            maxBounces--;
            if (maxBounces <= 0)
            {
                exploded = true;
                FadeOut();
            }
        }
    }
}
