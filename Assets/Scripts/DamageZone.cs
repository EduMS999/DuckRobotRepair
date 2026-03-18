using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damageAmount = -1; // Cantidad de vida que restara por vez que choque con la trampa

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("Object that entered the trigger: " + other);

        PlayerController PlayerController = other.GetComponent<PlayerController>();

        if (PlayerController.health <= PlayerController.maxHealth && PlayerController != null)
        {
            PlayerController.ChangeHealth(damageAmount);
        }

    }
}
