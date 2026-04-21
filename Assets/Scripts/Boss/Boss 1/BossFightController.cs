using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 2500f;
    private float currentHealth;
    private float healthPercentage;
    private bool isInvencible = false;
    public float shootForce = 5f;

    [Header("Recursos")]
    public GameObject projectile;
    public GameObject robot;
    public GameObject trampa;

    PatrullaBoss iniciarPatrulla;
    Rigidbody2D rb;
    Animator animator;

    [Header("Efectos Visuales")]
    // Particle systems para efectos visuales
    public ParticleSystem smokeParticleEffect;
    public ParticleSystem fixedParticleEffect;

    [Header("Spawn Points")]
    public GameObject[] spawnPoints; // Array de puntos de spawn para los robots 

    //Controlar tiempos entre rafagas
    private bool isShooting = false;
    private float shootCooldown = 1.5f;
    private float shootTimer = 0f;

    //Controlar tiempos de spawn de robots
    private bool isSpawning = false;
    private float spawnCooldown = 3f;
    private float spawnTimer = 0f;

    private int currentPhase = 0;
    private bool isInPhaseTransition = false;
    private bool trapsEnabled = false;

    public bool isDead = false;

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
        shootTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (healthPercentage <= 1f && healthPercentage > 0.66f)
        {
            // Fase 1: Solo lanza proyectiles
            CheckPhaseTransition(1);
            if (isInPhaseTransition)
                return; // no ataca durante la transición
            if (Random.Range(0f, 10f) < 0.01f) // Probabilidad de disparar cada frame (ajustable)
            {
                ShootAtPlayer();
            }

            if(shootTimer >= shootCooldown && !isShooting)
            {
                StartCoroutine(ShootAngledProjectiles());
                shootTimer = 0f;
            }
        }
        else if(healthPercentage <= 0.66f && healthPercentage > 0.33f)
        {
            // Fase 2: Fase 1 + spawnea robots que persiguen
            CheckPhaseTransition(2);
            if (isInPhaseTransition)
                return; // no ataca durante la transición
            if (Random.Range(0f, 10f) < 0.01f) // Probabilidad de disparar cada frame (ajustable)
            {
                ShootAtPlayer();
            }

            if (shootTimer >= shootCooldown && !isShooting)
            {
                StartCoroutine(ShootAngledProjectiles());
                shootTimer = 0f;
            }
            if(spawnTimer >= spawnCooldown && !isSpawning)
            {
                StartCoroutine(SpawnRobotsWithDelay());
                spawnTimer = 0f;
            }
        }
        else if(healthPercentage <= 0.33f && healthPercentage > 0f)
        {
            // Fase 3: Fase 2 + paneles de daño en los dos lados, lanzamientos mas agresivos y velocidad aumentada
            CheckPhaseTransition(3);
            if (isInPhaseTransition)
                return; // no ataca durante la transición
            if (Random.Range(0f, 10f) < 0.01f) // Probabilidad de disparar cada frame (ajustable)
            {
                ShootAtPlayer();
            }

            if (shootTimer >= shootCooldown && !isShooting)
            {
                StartCoroutine(ShootAngledProjectiles());
                shootTimer = 0f;
            }
            if (spawnTimer >= spawnCooldown && !isSpawning)
            {
                StartCoroutine(SpawnRobotsWithDelay());
                spawnTimer = 0f;
            }
            CheckTramp(); 
        }
    }

    void CheckPhaseTransition(int newPhase)
    {
        if (currentPhase == newPhase) return; // ya estamos en esta fase, no hacer nada

        currentPhase = newPhase;
        StartCoroutine(StartPhase()); // solo se ejecuta una vez al cambiar de fase
    }
    void CheckTramp()
    {
        if (trapsEnabled) return; // ya activadas, no repetir

        trapsEnabled = true;
        StartCoroutine(EnableTraps());
    }

    IEnumerator EnableTraps()
    {
        while (trapsEnabled)
        {
            trampa.SetActive(true);
            yield return new WaitForSeconds(10f);
            trampa.SetActive(false);
            yield return new WaitForSeconds(3f); // pausa entre activaciones
        }
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
        if(isInvencible)
            return; // Si el boss es invencible, no recibe daño
        currentHealth -= damage;
        healthPercentage = currentHealth / maxHealth;
        UIHandler.instance.SetBossHealthValue(healthPercentage, 1);
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
        trapsEnabled = false; // Desactivar trampas al morir
        isDead = true;
    }


    IEnumerator SpawnRobotsWithDelay()
    {
        isSpawning = true;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Instantiate(robot, spawnPoints[i].transform.position, Quaternion.identity); 
            yield return new WaitForSeconds(2f); // Espera 2 segundos antes de spawnear el siguiente robot
        }
        isSpawning = false;
    }

    IEnumerator ShootAngledProjectiles()
    {
        isShooting = true;

        float angleStep = 30f;
        int shootingAngles = Mathf.RoundToInt(180f / angleStep); // 5 angulos de disparo (30, 60, 90, 120, 150)

        for (int i = 1; i < shootingAngles; i++) // Dispara proyectiles a 30 grados de separación, empezando desde 30 grados y acabando en 150 grados
        {
            float angle = i * angleStep + 180;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 spawnPos = transform.position + Vector3.up * 2f;
            GameObject bossProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
            bossProjectile.GetComponent<BossProjectile>().Launch(direction, shootForce);

            yield return new WaitForSeconds(0.1f); // Espera 5 segundos antes de disparar el siguiente conjunto de proyectiles
        }

        isShooting = false;
    }

    void ShootAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Vector2 spawnPos = transform.position + Vector3.up * 2f;
            GameObject bossProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
            bossProjectile.GetComponent<BossProjectile>().Launch(direction, shootForce);
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

    IEnumerator StartPhase()
    {
        isInPhaseTransition = true;
        isInvencible = true;
        animator.SetBool("startPhase", true);

        yield return new WaitForSeconds(5f);

        animator.SetBool("startPhase", false);
        isInvencible = false;
        isInPhaseTransition = false; // ahora sí puede atacar
    }


}
