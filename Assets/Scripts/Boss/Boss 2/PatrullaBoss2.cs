using UnityEngine;

public class PatrullaBoss2 : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2.0f;
    public float waitTime = 1.0f; // Tiempo de espera en cada esquina

    private int currentWaypointIndex = 0;
    private float waitCounter = 0;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        // Calcular la dirección y distancia
        Transform target = waypoints[currentWaypointIndex];

        // Si no hemos llegado al punto, nos movemos
        if (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            // Moverse hacia el objetivo
            rb.MovePosition(Vector2.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime));
        }
        else
        {
            // Si ya llegamos, esperamos un poco y pasamos al siguiente
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                // El operador % (módulo) hace que al llegar al 4, vuelva al 0 automáticamente
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                waitCounter = 0;
            }
        }
    }
}
