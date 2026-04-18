using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrullaBoss : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;
    private bool moviendoDerecha = true;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Determinamos la dirección de la velocidad
        float velocidadActual = moviendoDerecha ? velocidad : -velocidad;

        // Aplicamos la velocidad
        rb.linearVelocity = new Vector2(velocidadActual, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si tocamos un objeto con la etiqueta de límite, damos la vuelta
        if (collision.CompareTag("Ruta"))
        {
            // Invertimos la variable booleana
            moviendoDerecha = !moviendoDerecha;
        }
    }
}