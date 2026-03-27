using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force, ForceMode2D.Impulse); // Aplica una fuerza instantánea al proyectil en la dirección dada
        Destroy(gameObject, 6f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ChangeHealth(-1); // Reduce la salud del jugador en 1 si colisiona con el proyectil
                if (!player.invincible)
                    Destroy(gameObject); // Si el jugador es invencible, no se destruye el proyectil
            }
        }
    }
}
