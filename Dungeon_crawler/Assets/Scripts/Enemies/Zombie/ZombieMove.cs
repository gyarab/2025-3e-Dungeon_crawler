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

        if (currentState != State.Following)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            FollowPlayer();
        }
        else
        {
            StartCoroutine(AttackRoutine());
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    IEnumerator AttackRoutine()
    {
        currentState = State.Attacking;

        rb.linearVelocity = Vector2.zero;

        dashDirection = (player.position - transform.position).normalized;

        if (dashDirection.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dashDirection.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetTrigger("WindUpAttack");

        yield return new WaitForSeconds(windUpTime);

        animator.SetTrigger("Attack");

        float timer = 0f;
        while (timer < dashTime)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;

        currentState = State.Cooldown;

  
        yield return new WaitForSeconds(cooldownTime);

        currentState = State.Following;
    }
}