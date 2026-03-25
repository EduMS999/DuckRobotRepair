using UnityEngine;

public class Projectile : MonoBehaviour
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
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null) 
        { 
            enemy.Fix();
            Destroy(gameObject); // Destruye el proyectil si colisiona con un enemigo
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("World"))
        {
            Destroy(gameObject); // Destruye el proyectil si colisiona con los colliders del tilemap
        }
    }
}
