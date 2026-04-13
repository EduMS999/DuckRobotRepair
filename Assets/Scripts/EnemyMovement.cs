using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{

    [Header("Configuración de Persecución")]
    private Transform player;           // Referencia al transform del jugador.
    public float chaseSpeed = 3.5f;       // Velocidad cuando te persigue
    public float detectionRange = 9999f;  // Distancia a la que el enemigo detecta al jugador.
    public float loseRange = 9999f;      // Distancia para salir del modo persecución (Histéresis)
    public float stoppingDistance = 0f; // Distancia a la que el enemigo se detendrá del jugador.

    [Header("Estado Interno")]
    private bool isChasing;           // Nueva bandera para saber si el enemigo está persiguiendo al jugador.

    [Header("Waypoints del lago")]
    private Transform[] lakeWaypoints; // 4 esquinas del lago en el Inspector

    private Transform currentWaypoint;
    private bool isNavigatingAroundLake;

    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private EnemyController enemyController;

    // Se ejecuta al iniciar el juego
    void Start()
    {
        // Obtenemos las referencias a los componentes del mismo GameObject
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Consigue el transform del jugador usando su tag al spawnear

        // Almacena los waypoints del lago en un array para fácil acceso
        GameObject lago = GameObject.Find("Lago");
        lakeWaypoints = new Transform[lago.transform.childCount];
        for (int i = 0; i < lago.transform.childCount; i++)
        {
            lakeWaypoints[i] = lago.transform.GetChild(i);
        }
    }

    void FixedUpdate()
    {
        // Si el robot ya ha sido reparado, detenemos cualquier lógica de movimiento
        if (enemyController != null && !enemyController.isBroken)
        {
            return;
        }

        if (player == null) return;

        // Calculamos la distancia actual entre el enemigo y el jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- 1. LÓGICA DE TRANSICIÓN (HISTÉRESIS) ---
        if (!isChasing && distanceToPlayer <= detectionRange)
        {
            // Entra en persecución
            isChasing = true;;
            StopAllCoroutines();
        }
        else if (isChasing && distanceToPlayer > loseRange)
        {
            // Solo deja de perseguir si supera el loseRange
            isChasing = false;
        }

        // --- 2. EJECUCIÓN BASADA EN EL ESTADO ---
        if (isChasing)
        {
            ModoPersecucion(distanceToPlayer);
        }
        else
        {
            
        }
    }

    // Lógica específica para seguir al jugador
    void ModoPersecucion(float currentDistance)
    {
        Vector2 direction = Vector2.zero;

        // --- LÓGICA DE MOVIMIENTO DETENIDO AL DISPARAR ---
        // Solo se mueve si NO está en rango de disparo Y no ha llegado a la distancia de parada
        if (currentDistance > stoppingDistance)
        {
            direction = CalcularDireccion();
            //direction = (Vector2)player.position - rigidbody2d.position;
            //direction.Normalize();
            rigidbody2d.MovePosition(rigidbody2d.position + direction * chaseSpeed * Time.fixedDeltaTime);
          // pasamos esta dirección al animador pero NO al MovePosition.
        }

        ActualizarAnimador(direction);
    }

    // Centralizamos la actualización de animaciones para evitar repetir código
    void ActualizarAnimador(Vector2 direction)
    {
        if (animator != null)
        {
            animator.SetFloat("Move X", direction.x);
            animator.SetFloat("Move Y", direction.y);
        }
    }

    Vector2 CalcularDireccion()
    {
        // Si hay camino directo al jugador, va directo
        if (!HayObstaculoHaciaJugador())
        {
            isNavigatingAroundLake = false;
            currentWaypoint = null;
            return ((Vector2)player.position - rigidbody2d.position).normalized;
        }

        // Si llegó al waypoint actual, elige el siguiente
        if (currentWaypoint != null)
        {
            float distToWaypoint = Vector2.Distance(transform.position, currentWaypoint.position);
            if (distToWaypoint < 0.5f)
                currentWaypoint = SiguienteWaypoint();
        }
        else
        {
            currentWaypoint = WaypointMasCercano();
        }

        return ((Vector2)currentWaypoint.position - rigidbody2d.position).normalized;
    }

    bool HayObstaculoHaciaJugador()
    {
        LayerMask obstacleLayer = LayerMask.GetMask("World");
        Vector2 dirToPlayer = ((Vector2)player.position - rigidbody2d.position).normalized;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleLayer);
        return hit.collider != null;
    }

    Transform WaypointMasCercano()
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform waypoint in lakeWaypoints)
        {
            float dist = Vector2.Distance(transform.position, waypoint.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = waypoint;
            }
        }

        return closest;
    }

    Transform SiguienteWaypoint()
    {
        // Elige el waypoint más cercano al jugador que no tenga obstáculo
        Transform best = null;
        float minDistToPlayer = float.MaxValue;
        LayerMask obstacleLayer = LayerMask.GetMask("World");

        foreach (Transform waypoint in lakeWaypoints)
        {
            if (waypoint == currentWaypoint) continue;

            float distToPlayer = Vector2.Distance(waypoint.position, player.position);
            Vector2 dir = ((Vector2)waypoint.position - rigidbody2d.position).normalized;
            float distToWaypoint = Vector2.Distance(transform.position, waypoint.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distToWaypoint, obstacleLayer);

            if (hit.collider == null && distToPlayer < minDistToPlayer)
            {
                minDistToPlayer = distToPlayer;
                best = waypoint;
            }
        }

        return best ?? WaypointMasCercano();
    }


}
