using System.Collections;
using UnityEngine;

public class MiniBossController : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 500f;
    private float currentHealth;
    private float healthPercentage;
    private bool isInvencible = false;
    public float shootForce = 10f;

    [Header("Recursos")]
    public GameObject projectile;

    Rigidbody2D rb;
    Animator animator;

    public bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShootAtPlayer());
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    IEnumerator ShootAtPlayer()
    {
        while (!isDead)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                Vector2 spawnPos = transform.position + Vector3.up * 2f;
                GameObject miniBossProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
                miniBossProjectile.GetComponent<MiniBossProjectile>().Launch(direction, shootForce);
            }
            yield return new WaitForSeconds(1f);
        }
           
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            TakeDamage(50f); // Hace daño al miniboss cada vez que es golpeado por un proyectil del jugador
            Destroy(other.gameObject); // Destruir el proyectil al impactar
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthPercentage = currentHealth / maxHealth;
        //UIHandler.instance.SetBossHealthValue(healthPercentage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Fixed");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f);
        GameManager.instance.miniBossesDead++; // Notificar al GameManager que el miniboss ha sido derrotado
    }
}
