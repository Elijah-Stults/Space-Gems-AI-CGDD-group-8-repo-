using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody rb;

    public float mouseSensitivity = 0.3f;
    private float mouseX;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        mouseX += Mouse.current.delta.ReadValue().x;
    }

    //Movement code 
    private void FixedUpdate()
    {
        float hor = 0f;
        float ver = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            hor = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            hor = 1f;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            ver = 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            ver = -1f;
        }

        Vector3 move = new Vector3(hor, 0f, ver);

        Vector3 moveDir = transform.TransformDirection(move);

        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX * mouseSensitivity, 0f));
        mouseX = 0f;
    }
}
