using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private int damage;
    [SerializeField] private int knockbackForce;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private List<string> unhittableTags = new List<string>();
    [SerializeField] public List<EffectSO> effectsOnHit = new List<EffectSO>();

    private HashSet<GameObject> ignore = new HashSet<GameObject>();

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetKnockbackForce(int knockbackForce)
    {
        this.knockbackForce = knockbackForce;
    }
    public void SetUnhittableTags(List<string> tags)
    {
        this.unhittableTags = tags;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //when collides with something, checks if it can be damaged, and if so, applies damage and knockback
        if (ignore.Contains(other.gameObject)) return;

        for (int i = 0; i < unhittableTags.Count; i++)
        {
            if (other.tag == unhittableTags[i])
            {
                return;
            }
        }

        if (!other.TryGetComponent(out Health health))
        {
            ignore.Add(other.gameObject);
            return;
        }

        bool damaged = health.TakeDamage(damage);
        //apply effects
        if (damaged) {
            foreach (EffectSO effect in effectsOnHit)
            {
                EffectInstance instance = other.gameObject.AddComponent<EffectInstance>();
                instance.effectData = effect;
            }
        }

        if (other.TryGetComponent(out Rigidbody2D rb) && damaged)
        {
            Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
            rb.AddForce(knockbackDir * knockbackForce*100, ForceMode2D.Impulse);
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
