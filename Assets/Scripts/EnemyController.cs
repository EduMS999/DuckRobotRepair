using System.Collections;
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
    public ParticleSystem destroyedParticleEffect;

    public GameObject[] healthCollectibles; // Array de objetos de salud que se activarán al ser reparado

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
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.ChangeHealth(-1);
            }
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
        int random = Random.Range(0, 100);
        if(random < 20)
        {
            int randomIndex = Random.Range(0, healthCollectibles.Length);
            Instantiate(healthCollectibles[randomIndex], transform.position + Vector3.up * 2f, Quaternion.identity);
        }
        
        StartCoroutine(DestroyEnemy());

    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        Instantiate(destroyedParticleEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
    }
}
