using System.Collections;
using UnityEngine;

public class BossFightController2 : MonoBehaviour
{
    [Header("Estadisticas")]
    public float maxHealth = 2500f;
    private float currentHealth;
    private float healthPercentage;
    private bool isInvencible = false;

    [Header("Recursos")]
    public GameObject projectile;
    public GameObject robot;
    public GameObject[] trampas;

    PatrullaBoss2 iniciarPatrulla;
    Rigidbody2D rb;
    Animator animator;

    [Header("Spawn Points")]
    public GameObject[] spawnPoints; // Array de puntos de spawn para los robots 
    public GameObject[] miniBossSpawnPoints; // Array de puntos de spawn para los mini bosses

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
        iniciarPatrulla = gameObject.GetComponent<PatrullaBoss2>();
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

            if (shootTimer >= shootCooldown && !isShooting)
            {
                StartCoroutine(ShootAngledProjectiles());
                shootTimer = 0f;
            }
        }
        else if (healthPercentage <= 0.66f && healthPercentage > 0.33f)
        {
            // Fase 2: Fase 1 + spawnea robots que persiguen + velocidad aumentada
            CheckPhaseTransition(2);
            if (isInPhaseTransition)
                return; // no ataca durante la transición
            if (Random.Range(0f, 10f) < 0.02f) // Probabilidad de disparar cada frame (ajustable)
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
        }
        else if (healthPercentage <= 0.33f && healthPercentage > 0f)
        {
            // Fase 3: Fase 2 + paneles de daño en varios lugares que aparecen y desaparecen, lanzamientos mas agresivos y velocidad aumentada
            CheckPhaseTransition(3);
            if (isInPhaseTransition)
                return; // no ataca durante la transición
            if (Random.Range(0f, 10f) < 0.02f) // Probabilidad de disparar cada frame (ajustable)
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

        if(currentPhase == 2)
        {
            iniciarPatrulla.speed += 1f; // Aumenta la velocidad en la fase 2
            SpawnMiniBosses(); // Spawnea mini bosses al entrar en la fase 2
        }
        else if(currentPhase == 3)
        {
            iniciarPatrulla.speed += 0.5f; // Aumenta aún más la velocidad en la fase 3
        }
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
            int randomIndex = Random.Range(0, trampas.Length);
            trampas[randomIndex].SetActive(true);
            yield return new WaitForSeconds(4f);
            trampas[randomIndex].SetActive(false);
            yield return new WaitForSeconds(1f); // pausa entre activaciones
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
        if (isInvencible)
            return; // Si el boss es invencible, no recibe daño
        currentHealth -= damage;
        healthPercentage = currentHealth / maxHealth;
        UIHandler.instance.SetBossHealthValue(healthPercentage);
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
        trapsEnabled = false; // Desactivar trampas al morir
        isDead = true;
    }


    IEnumerator SpawnRobotsWithDelay()
    {
        isSpawning = true;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Instantiate(robot, spawnPoints[i].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f); // Espera 1 segundo antes de spawnear el siguiente robot
        }
        isSpawning = false;
    }

    IEnumerator ShootAngledProjectiles()
    {
        isShooting = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { isShooting = false; yield break; }

        // Ángulo base apuntando al jugador
        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        float angleStep = 30f;
        int shootingAngles = Mathf.RoundToInt(180f / angleStep); // 5 angulos de disparo (30, 60, 90, 120, 150)
        float spreadOffset = -(shootingAngles / 2f) * angleStep; // centra el abanico en el jugador

        for (int i = 0; i < shootingAngles; i++)
        {
            float angle = baseAngle + spreadOffset + (i * angleStep);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 spawnPos = transform.position + Vector3.up * 2f;
            GameObject bossProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
            bossProjectile.GetComponent<BossProjectile>().Launch(direction, 5f);

            yield return new WaitForSeconds(0.1f);
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
            bossProjectile.GetComponent<BossProjectile>().Launch(direction, 10f);
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
        DesactivarPatrulla();
        isInPhaseTransition = true;
        isInvencible = true;
        animator.SetBool("startPhase", true);

        yield return new WaitForSeconds(5f);

        IniciarPatrulla();
        animator.SetBool("startPhase", false);
        isInvencible = false;
        isInPhaseTransition = false; // ahora sí puede atacar
    }

    void SpawnMiniBosses()
    {
        for (int i = 0; i < miniBossSpawnPoints.Length; i++)
        {
            Instantiate(robot, miniBossSpawnPoints[i].transform.position, Quaternion.identity);
        }
    }
}

