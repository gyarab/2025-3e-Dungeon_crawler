using UnityEngine;
using System.Collections;

public class ZombieMove : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 1f;

    [SerializeField] private Animator animator;
    [SerializeField] private float attackDistance = 2.5f;

    [Header("Dash Settings")]
    [SerializeField] private float windUpTime = 0.8f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float cooldownTime = 1.5f;

    private Rigidbody2D rb;

    private enum State { Following, Attacking, Cooldown }
    private State currentState = State.Following;

    private Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (currentState != State.Following) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            FollowPlayer();
        }
        else
        {
            // If close enough, start attack routine
            StartCoroutine(AttackRoutine());
        }
    }

    void FollowPlayer()
    {
        // Move towards the player, flip sprite based on direction
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (direction.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    IEnumerator AttackRoutine()
    {
        currentState = State.Attacking;

        // Stop Zombie Movement
        rb.linearVelocity = Vector2.zero;

        dashDirection = (player.position - transform.position).normalized;

        if (dashDirection.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (dashDirection.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Attack wind up
        animator.SetTrigger("WindUpAttack");

        yield return new WaitForSeconds(windUpTime);

        // Start attack dash animation
        animator.SetTrigger("Attack");

        float timer = 0f;

        // Dash towards the player for the specified dash time
        while (timer < dashTime)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.linearVelocity = Vector2.zero;

        // Start cooldown
        currentState = State.Cooldown;
        yield return new WaitForSeconds(cooldownTime);

        // Return to following state
        currentState = State.Following;
    }
}