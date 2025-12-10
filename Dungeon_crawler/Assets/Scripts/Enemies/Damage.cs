using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private int damage;
    [SerializeField] private int knockbackForce;
    [SerializeField] private bool destroyOnHit = false;

    private HashSet<GameObject> ignore = new HashSet<GameObject>();

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetKnockbackForce(int knockbackForce)
    {
        this.knockbackForce = knockbackForce;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (ignore.Contains(other.gameObject)) return;

        if (!other.TryGetComponent(out Health health))
        {
            ignore.Add(other.gameObject);
            return;
        }

        bool damaged = health.TakeDamage(damage);

        if (other.TryGetComponent(out Rigidbody2D rb) && damaged)
        {
            Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
