using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject bodyMesh;
    [SerializeField] private GameObject handsMesh;
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private Animator handsAnimator;
    [SerializeField] private Renderer handsRenderer;


    private float speed;
    [HideInInspector] public float speedDebuff;

    [Header("Abilities")]
    [SerializeField] private bool dashEnabled = true;
    [SerializeField] private float dashForce = 100f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;
    private bool isDashing;

    [HideInInspector] public int flip = 1;
    [HideInInspector] public UnityEvent<int> flipped;

    private Rigidbody2D rb;
    private Vector2 movementDir;
    private Vector2 velocity = Vector2.zero;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction dashAction;

    private bool canMove = true;
    public HashSet<Transform> moveBlockers = new HashSet<Transform>();

    private float lastMoveX = 1f;
    private bool hasInput;

    public bool canAttack = true;
    private Vector2 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Dash"];

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        dashAction.performed += OnDash;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDashing)
        {
            movementDir = context.ReadValue<Vector2>();
            return;
        }

        movementDir = context.ReadValue<Vector2>();

        hasInput = movementDir.sqrMagnitude > 0.01f;

        if (Mathf.Abs(movementDir.x) > 0.1f)
        {
            lastMoveX = Mathf.Sign(movementDir.x);
        }
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
        canMove = moveBlockers.Count>0 ? false : true;

        speed = Input.GetKey(KeyCode.LeftShift)
            ? runSpeed / (1 + speedDebuff)
            : walkSpeed / (1 + speedDebuff);

        if (!canMove || isDashing)
        {
            return;
        }

        Vector2 input = isDashing ? Vector2.zero : movementDir;
        Vector2 target = input * speed;

        if (input.sqrMagnitude < 0.01f)
        {
            target = Vector2.zero;
        }

        rb.linearVelocity = Vector2.SmoothDamp(
            rb.linearVelocity,
            target,
            ref velocity,
            smoothTime
        );
    }

    void Update()
    {
        if (isDashing)
        {
            bodyAnimator.SetFloat("Speed", 0f);
            handsAnimator.SetFloat("Speed", 0f);
            bodyAnimator.SetFloat("MoveX", lastMoveX);
            handsAnimator.SetFloat("MoveX", lastMoveX);
            return;
        }

        if (Mathf.Abs(movementDir.x) > 0.1f)
        {
            float dir = Mathf.Sign(movementDir.x);
            if (dir != flip)
            {
                Flip(dir);
            }
        }

        bodyAnimator.SetFloat("Speed", hasInput ? rb.linearVelocity.magnitude : 0f);
        handsAnimator.SetFloat("Speed", hasInput ? rb.linearVelocity.magnitude : 0f);
        bodyAnimator.SetFloat("MoveX", lastMoveX);
        handsAnimator.SetFloat("MoveX", lastMoveX);
    }

    public void Flip(float dir)
    {
        if (isDashing) return;

        flip = (int)Mathf.Sign(dir);
        flipped.Invoke(flip);
        bodyMesh.transform.localScale = new Vector3(flip, 1, 1);
        handsMesh.transform.localScale = new Vector3(flip, 1, 1);
    }

    private void Dash()
    {
        bodyAnimator.SetTrigger("Dash");
        isDashing = true;
        handsRenderer.enabled = false;


        dashDirection = movementDir.sqrMagnitude > 0.01f
            ? movementDir.normalized
            : new Vector2(flip, 0);

        rb.AddForce(dashDirection * speed * dashForce, ForceMode2D.Impulse);

        StartCoroutine(DashCooldown());
        StartCoroutine(DashEnd());
    }

    IEnumerator DashEnd()
    {
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity *= 0.2f;

        isDashing = false;
        

    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        handsRenderer.enabled = true;
        canDash = true;
    }
}