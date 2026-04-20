using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrullaMiniBoss : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;
    private bool moviendoAbajo = true;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Determinamos la dirección de la velocidad
        float velocidadActual = moviendoAbajo ? velocidad : -velocidad;

        // Aplicamos la velocidad
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadActual);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si tocamos un objeto con la etiqueta de límite, damos la vuelta
        if (collision.CompareTag("Ruta"))
        {
            // Invertimos la variable booleana
            moviendoAbajo = !moviendoAbajo;
        }
    }
}