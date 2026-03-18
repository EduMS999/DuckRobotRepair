using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int amountHealed = 1; // Cantidad de vida que sumara por collectible

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Object that entered the trigger: " + other);

        PlayerController PlayerController = other.GetComponent<PlayerController>();

        if(PlayerController.health < PlayerController.maxHealth && PlayerController != null)
        {
            PlayerController.ChangeHealth(amountHealed);
            Destroy(gameObject);   
        }
        
    }
}
