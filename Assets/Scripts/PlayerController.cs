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
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    // Variables relacionadas con la invencibilidad temporal
    public float timeInvincible = 2.0f; 
    bool isInvincible; 
    float damageCooldown;

    public GameObject projectilePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input = GetComponent<PlayerInput>();    
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
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

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y); 
        animator.SetFloat("Speed", move.magnitude);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y); 
            moveDirection.Normalize();
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
            animator.SetTrigger("Hit");
            isInvincible = true; 
            damageCooldown = timeInvincible; 
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    public void Launch(InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 1.25f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>(); 
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
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
