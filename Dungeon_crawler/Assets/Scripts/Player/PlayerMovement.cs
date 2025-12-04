using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private Animator animator;
    private float speed;
    [HideInInspector]
    public float speedDebuff;
    [Header("Abilities")]
    [SerializeField] private bool dashEnabled = true;
    [SerializeField] private float dashForce = 100f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    [HideInInspector] public bool isFlipped = false;
    [HideInInspector] public UnityEvent<bool> flipped;

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

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDir = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash && dashEnabled)
        {
            Dash();
        }
    }

    void FixedUpdate()
    {
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed / (1 + speedDebuff) : walkSpeed / (1 + speedDebuff);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, movementDir * speed, ref velocity, smoothTime);
    }

    void Update()
    {
        
        if (movementDir.x != 0)
        {
            int flip = movementDir.x > 0 ? 1 : -1;

            playerMesh.transform.localScale = new Vector3(1 * flip, 1, 1);

            if (isFlipped != (flip == -1))
            {
                //true if flipped left
                //false if flipped right
                flipped.Invoke(flip == -1);
            }
            isFlipped = flip == -1;
        }

        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetFloat("MoveX", movementDir.x);
    }

    void Dash()
    {
        rb.AddForce(movementDir.normalized * speed * dashForce, ForceMode2D.Impulse);
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


}
