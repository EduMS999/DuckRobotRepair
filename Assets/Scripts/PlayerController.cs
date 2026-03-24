using System;
using System.Threading;
using Beginner2D;
using UnityEditor.Build;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Variables related to player character movement
    public PlayerInput Input;
    Rigidbody2D rb;
    Vector2 move;
    public float movementSpeed = 3.0f;

    // Variables related to the health system
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    // Variables related to temporary invincibility
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    // Variables related to Animation
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    // Variables related to Projectile 
    public GameObject projectilePrefab;

    // Variables related to NPC
    private NonPlayerCharacter lastNonPlayerCharacter;
    private RaycastHit2D hit;


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


        hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            // Si el rayo toca algo, obtenemos su componente NPC
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            if (npc == null) return; // Si el objeto no tiene un NPC, no hacemos nada
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
        }
        else
        {
            if(lastNonPlayerCharacter != null)
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
        }

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
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    public void Launch(InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 1.25f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>(); 
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if(hit.collider != null)
        {
            UIHandler.instance.DisplayDialogue();
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
    }
}
