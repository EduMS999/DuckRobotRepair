using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyController : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;

    public bool vertical;

    public float changeTime = 3.0f;
    float timer = 0;
    int direction = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = changeTime;
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;

        if (vertical) 
        { 
            position.y = position.y + speed * direction * Time.deltaTime; 
        } 
        else 
        { 
            position.x = position.x + speed * direction * Time.deltaTime; 
        }
        rb.MovePosition(position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0) 
        { 
            direction = -direction; 
            timer = changeTime; 
            //vertical = !vertical;
        }
    }
}
