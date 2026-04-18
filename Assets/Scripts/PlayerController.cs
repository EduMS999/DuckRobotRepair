using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    // Caches de parámetros para evitar string lookups cada frame
    private static readonly int HashLookX = Animator.StringToHash("Look X");
    private static readonly int HashLookY = Animator.StringToHash("Look Y");
    private static readonly int HashShootX = Animator.StringToHash("Shoot X");
    private static readonly int HashShootY = Animator.StringToHash("Shoot Y");
    private static readonly int HashLaunch = Animator.StringToHash("Launch");
    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashSpeed = Animator.StringToHash("Speed");

    // Variables related to player character movement
    PlayerInput Input;
    Rigidbody2D rb;
    Vector2 move;
    public float movementSpeed = 3.0f;
    public float dashDistance = 3f; // Valor optimo
    public float dashDuration = 0.2f; // Valor optimo
    private bool isDashing = false;
    private TrailRenderer trailRenderer;
    private bool canDash = true; // Variable para controlar si el jugador puede dashear o no
    public float dashCooldownTime = 1.0f; // Tiempo de cooldown entre dashes
    private float dashCooldown; // Tiempo de cooldown entre dashes

    // Variables related to the health system
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    // Variables related to temporary invincibility
    public float timeInvincible = 1.0f;
    bool isInvincible;
    public bool invincible { get { return isInvincible; } }
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
        trailRenderer = GetComponentInChildren<TrailRenderer>();

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

        if(!canDash)
        {
            dashCooldown -= Time.deltaTime;
            if (dashCooldown < 0)
                canDash = true;
        }

        animator.SetFloat(HashLookX, moveDirection.x);
        animator.SetFloat(HashLookY, moveDirection.y); 
        animator.SetFloat(HashSpeed, move.magnitude);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y); 
            moveDirection.Normalize();
        }

    }

    void FixedUpdate() 
    {
        if (isDashing)
            return; // Impide que el personaje se mueva normal mientras dashea

        Vector2 position = (Vector2)rb.position + move * movementSpeed * Time.deltaTime; 
        rb.MovePosition(position);
    }

    public void ChangeHealth (int amount)
    {
        if (amount < 0) 
        { 
            if (isInvincible) return;
            animator.SetTrigger(HashHit);
            isInvincible = true; 
            damageCooldown = timeInvincible; 
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    public void Launch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 spawnPosition = rb.position + Vector2.up * 1.25f; // Posición de spawn del proyectil, ligeramente por encima del jugador
            GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos) + new Vector3(0,0,10); // Obtenemos el punto en el mundo donde se encuentra el mouse
            Vector2 direction = ((Vector2)worldPoint - spawnPosition).normalized; // normalizamos la dirección del proyectil para que tenga una longitud de 1
            //Debug.Log($"Mouse Position: {mousePos}, World Point: {worldPoint}, Direction: {direction}");
            projectile.Launch(direction, 20);
            animator.SetFloat(HashShootX, direction.x);
            animator.SetFloat(HashShootY, direction.y);
            animator.SetTrigger(HashLaunch);
        }
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

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            //Debug.Log("Dash");
            StartCoroutine(DoDash());
            //rb.AddForce(move * 100, ForceMode2D.Impulse);
        }
    }

    IEnumerator DoDash()
    {
        if (!canDash) yield break; // Si el jugador no puede dashear, salimos de la corrutina
        isDashing = true;
        dashCooldown = dashCooldownTime; // Reiniciamos el cooldown
        canDash = false; // Desactivamos la posibilidad de dashear hasta que el cooldown termine
        trailRenderer.enabled = true;
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = (Vector2)transform.position + (moveDirection * dashDistance);
        float elapsed = 0;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / dashDuration;
                
            // Mueve el rigidbody físicamente hacia el destino
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, percent));
            yield return null;
        }

        trailRenderer.enabled = false;
        isDashing = false;
    }
}

