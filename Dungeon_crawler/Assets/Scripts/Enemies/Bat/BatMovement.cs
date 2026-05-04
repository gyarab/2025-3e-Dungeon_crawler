using System.Collections;
using UnityEngine;

public class BatMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float circleSpeed = 2f;
    [SerializeField] private float circleRadius = 2.5f;

    [Header("Behavior")]
    [SerializeField] private float startCircleDistance = 2f;
    [SerializeField] private float circleDurationMin = 2f;
    [SerializeField] private float circleDurationMax = 4f;
    [SerializeField] private float preAttackPause = 0.5f;
    [SerializeField] private float lungeSpeed = 8f;
    [SerializeField] private float lungeDuration = 0.5f;
    [SerializeField] private float recoveryTime = 1.5f;

    void Start()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            // Fly towards player until close enough
            while (Vector2.Distance(transform.position, player.position) > startCircleDistance)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

                FacePlayer();
                yield return null;
            }

            // Circle player for random duration within constraints
            float circleDuration = Random.Range(circleDurationMin, circleDurationMax);
            float timer = 0f;

            Vector2 offset = (transform.position - player.position).normalized;
            float angle = Mathf.Atan2(offset.y, offset.x);

            while (timer < circleDuration)
            {
                angle += circleSpeed * Time.deltaTime;

                Vector2 newOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;
                transform.position = (Vector2)player.position + newOffset;

                timer += Time.deltaTime;

                FacePlayer();
                yield return null;
            }

            // Stop movement before attacking
            Vector2 lungeDir = (player.position - transform.position).normalized;

            float pauseTimer = preAttackPause;
            while (pauseTimer > 0f)
            {
                pauseTimer -= Time.deltaTime;
                FacePlayer();
                yield return null;
            }

            // Attack towards the player
            animator.SetTrigger("Attack");

            float lungeTimer = lungeDuration;
            while (lungeTimer > 0f)
            {
                transform.position += (Vector3)(lungeDir * lungeSpeed * Time.deltaTime);

                lungeTimer -= Time.deltaTime;
                yield return null;
            }

            // Recovery after attack, vulnerable to hits
            float recoveryTimer = recoveryTime;
            while (recoveryTimer > 0f)
            {
                recoveryTimer -= Time.deltaTime;
                yield return null;
            }
        }
    }

    void FacePlayer()
    {
        // Flip sprite based on player position
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}