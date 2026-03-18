using System;
using Beginner2D;
using UnityEditor.Build;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput Input;
    private Vector2 move; // Guarda los valores del movimiento
    public float movementSpeed;
    private Rigidbody2D rb;
    public int maxHealth = 5; 
    int currentHealth;
    public int health { get { return currentHealth; } }

    // Variables relacionadas con la invencibilidad temporal
    public float timeInvincible = 2.0f; 
    bool isInvincible; 
    float damageCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input = GetComponent<PlayerInput>();    
        rb = GetComponent<Rigidbody2D>();
        
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.actions["Move"].ReadValue<Vector2>();
        //Debug.Log(move); 


        /*RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            FindFriend(hit);
        }*/

        if (isInvincible) 
        { 
            damageCooldown -= Time.deltaTime; 
            if (damageCooldown < 0) 
                isInvincible = false; 
        }
    }

    void FixedUpdate() 
    {
        Vector2 position = (Vector2)rb.position + move * movementSpeed * Time.deltaTime; 
        rb.MovePosition(position);
    }

    public void ChangeHealth (int amount)
    {
        if (amount < 0) 
        { 
            if (isInvincible) return; 
            isInvincible = true; 
            damageCooldown = timeInvincible; 
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
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
