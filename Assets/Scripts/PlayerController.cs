using Beginner2D;
using UnityEditor.Build;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput Input;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input = GetComponent<PlayerInput>();    
        rb = GetComponent<Rigidbody2D>();  
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Input.actions["Move"].ReadValue<Vector2>();
        //Debug.Log(move); 
        Vector2 position = (Vector2)transform.position + move * 3f * Time.deltaTime; 
        transform.position = position;

        /*RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            FindFriend(hit);
        }*/
    }

    void FindFriend(RaycastHit2D hit)
    {
        //UIHandler.instance.DisplayDialogue();
    }

    /*public void Interact(InputAction.CallbackContext context)
    {
        // Lanzamos un rayo invisible (Raycast) para detectar NPCs en la capa "NPC"
        RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));

        if(hit.collider != null)
        {
            // Si el rayo toca algo, obtenemos su componente NPC y mostramos su dialogo
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend(hit); // Logica adicional para UI
        }
        else
        {
            // Si no tocamos nada, ocultamos el último dialogo abierto
            if(lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
            }
        }
    }*/
}
