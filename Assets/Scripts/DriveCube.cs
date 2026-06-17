using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DriveCube : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;

    public float moveSpeed = 2f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool isMoving = false;

        // Check legacy Input System
#if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM
        if (Input.GetKey(KeyCode.W))
        {
            isMoving = true;
        }
#endif

        // Check new Input System
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current[Key.W].isPressed)
        {
            isMoving = true;
        }
#endif

        // Move the cube forward if key is pressed
        if (isMoving)
        {
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
        }

        // Set Speed parameter based on movement state
        float speed = isMoving ? moveSpeed : 0f;
        anim.SetFloat("Speed", speed);
    }
}