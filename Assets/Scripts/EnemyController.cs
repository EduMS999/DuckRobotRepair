using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    Animator animator;

    public bool vertical;
    bool broken = true;
    public bool isBroken { get { return broken; } }

    public float changeTime = 3.0f;
    float timer = 0;
    int direction = 1;

    public ParticleSystem smokeParticleEffect;
    public ParticleSystem fixedParticleEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!broken)  
            return;

        Vector2 position = rb.position;

        if (vertical) 
        { 
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0); 
            animator.SetFloat("Move Y", direction);
        } 
        else 
        { 
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction); 
            animator.SetFloat("Move Y", 0);
        }
        rb.MovePosition(position);
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
        timer -= Time.deltaTime;

        if (timer < 0) 
        { 
            direction = -direction; 
            timer = changeTime; 
            //vertical = !vertical;
        }
    }

    public void Fix() 
    {
        animator.SetTrigger("Fixed");
        broken = false; 
        rb.simulated = false; 
        smokeParticleEffect.Stop();
        Instantiate(fixedParticleEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
    }
}
