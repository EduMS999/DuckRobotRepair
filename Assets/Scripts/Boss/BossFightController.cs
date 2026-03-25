using Newtonsoft.Json.Linq;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 1000f;
    private float currentHealth;
    private float healthPercentage;

    [Header("Recursos")]
    public GameObject projectile;
    public GameObject robot;

    PatrullaBoss iniciarPatrulla;
    Rigidbody2D rb;
    Animator animator;

    [Header("Efectos Visuales")]
    // Particle systems para efectos visuales
    public ParticleSystem smokeParticleEffect;
    public ParticleSystem fixedParticleEffect;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        iniciarPatrulla = gameObject.GetComponent<PatrullaBoss>();
        // Deshabilitar el script PatrullaBoss al iniciar la escena
        if (iniciarPatrulla.enabled)
        {
            iniciarPatrulla.enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthPercentage = currentHealth / maxHealth;
        IniciarPatrulla();
    }

    // Update is called once per frame
    void Update()
    {
        if(healthPercentage <= 1f && healthPercentage > 0.66f)
        {
            // Fase 1: Solo lanza proyectiles
            StartPhase();
        }
        else if(healthPercentage <= 0.66f && healthPercentage > 0.33f)
        {
            // Fase 2: Fase 1 + spawnea robots que persiguen con velocidades distintas
        }
        else if(healthPercentage <= 0.33f && healthPercentage > 0f)
        {
            // Fase 3: Fase 2 + paneles de daño en los dos lados, lanzamientos mas agresivos y velocidad aumentada
        }
    }

    void StartPhase()
    {
        // Añadir camara shake o algun efecto visual para indicar el cambio de fase
        // Añadir invencibilidad temporal al boss durante la transición de fase para evitar que el jugador abuse del daño durante el cambio de fase
    }

    void IniciarPatrulla()
    {
        iniciarPatrulla.enabled = true;
    }

    void DesactivarPatrulla()
    {
        iniciarPatrulla.enabled = false;
        rb.linearVelocity = Vector2.zero; // Detener el movimiento del boss
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthPercentage = currentHealth / maxHealth;
        //Debug.Log("Boss Health: " + currentHealth + "/" + maxHealth + " (" + (healthPercentage * 100) + "%)");  
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Fixed"); // Reutilizamos la animación de "Fixed" pero esta vez loopeada
        rb.simulated = false;
        smokeParticleEffect.Stop();
        ParticleSystem fpe = Instantiate(fixedParticleEffect, transform.position + Vector3.up * 7f, Quaternion.identity, transform);
        fpe.transform.localScale = Vector3.one * 9f; // Aumentar el tamaño del efecto de partículas para que sea más impresionante
    }

    void SpawnRobot()
    {
        Instantiate(robot, transform.position + Vector3.down * 2f, Quaternion.identity); // COMPLETAR
    }

    void ShootAngledProjectiles()
    {
        float angleStep = 30f;
        int shootingAngles = Mathf.RoundToInt(180f / angleStep); // 5 angulos de disparo (30, 60, 90, 120, 150)

        for (int i = 1; i < shootingAngles; i++) // Dispara proyectiles a 30 grados de separación, empezando desde 30 grados y acabando en 150 grados
        {
            float angle = i * angleStep +180;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 spawnPos = transform.position + Vector3.up * 2f;
            GameObject bossProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
            bossProjectile.GetComponent<BossProjectile>().Launch(direction, 25f);
        }
    }

    void ShootAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Vector2 spawnPos = transform.position + Vector3.up * 2f;
            GameObject bossProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
            bossProjectile.GetComponent<BossProjectile>().Launch(direction, 25f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            TakeDamage(50f); // Hace daño al boss cada vez que es golpeado por un proyectil del jugador
            Destroy(other.gameObject); // Destruir el proyectil al impactar
        }
    }
}
