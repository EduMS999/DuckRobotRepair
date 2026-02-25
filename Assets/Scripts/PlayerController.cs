using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput Input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input = GetComponent<PlayerInput>();    
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Input.actions["Move"].ReadValue<Vector2>();
        //Debug.Log(move); 
        Vector2 position = (Vector2)transform.position + move * 3f * Time.deltaTime; 
        transform.position = position;
    }

    public void Move(InputAction.CallbackContext context)
    {
       
        
        
        
    }
}
