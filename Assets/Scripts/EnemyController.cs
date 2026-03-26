using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    bool broken = true;
    public bool isBroken { get { return broken; } }

    public ParticleSystem smokeParticleEffect;
    public ParticleSystem fixedParticleEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    private void Update()
    {
 
    }

    public void Fix() 
    {
        animator.SetTrigger("Fixed");
        broken = false; 
        rb.simulated = false; 
        smokeParticleEffect.Stop();
        Instantiate(fixedParticleEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
        Destroy(gameObject, 3f);
    }
}
