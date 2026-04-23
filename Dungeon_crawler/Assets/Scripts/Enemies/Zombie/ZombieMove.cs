using UnityEngine;

public class ZombieMove : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float speed = 1f;

    [SerializeField] private Animator animator;
    [SerializeField] private float attackDistance = 5f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            FollowPlayer();
        } else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("WindUpAttack");
        }
    }

    void FollowPlayer()
    {
               if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
