using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float smoothTime = 0.1f;
    private float speed;
    [HideInInspector]
    public float speedDebuff;
    [Header("Abilities")]
    [SerializeField] private bool dash = true;
    [SerializeField] private float dashForce = 100f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;

    private Rigidbody2D rb;
    private Vector2 movementDir;
    private Vector2 velocity = Vector2.zero;

    // Input System
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction dashAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Dash"];

        moveAction.performed += OnMove;
        dashAction.performed += OnDash;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementDir = context.ReadValue<Vector2>();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            Dash();
        }
    }

    void FixedUpdate()
    {
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed / (1 + speedDebuff) : walkSpeed / (1 + speedDebuff);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, movementDir * speed, ref velocity, smoothTime);
    }

    void Dash()
    {
        rb.AddForce(movementDir.normalized* speed * dashForce, ForceMode2D.Impulse);
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


}
