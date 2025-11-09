using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spear : Weapon
{
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackOffset = 0.5f;
    [SerializeField] private float hitboxRange = 2f;
    [SerializeField] private float hitboxWidth = 0.5f;
    [SerializeField] private float attackReach = 4f;
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private int knockbackForce = 5;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;
    }


    public override bool OnAttack()
    {
        if (!base.OnAttack()) { return false; }

        GameObject hitbox = new GameObject("SwordHitbox");
        hitbox.transform.parent = transform;
        hitbox.transform.localRotation = Quaternion.identity;
        hitbox.transform.position = transform.position;
        BoxCollider2D collider = hitbox.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(hitboxRange, hitboxWidth);
        collider.isTrigger = true;

        Damage swordHitbox = hitbox.AddComponent<Damage>();
        swordHitbox.SetDamage(damage);
        swordHitbox.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(PikeSpear(attackDuration));

        return true;
    }


    IEnumerator PikeSpear(float duration)
    {
        bool returnToStart = !rotateWeapon;
        rotateWeapon = true;
        yield return new WaitForSeconds(0.05f);
        rotateWeapon = false;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int flip = mousePos.x < transform.parent.position.x ? -1 : 1;

        Vector3 direction = (mousePos - transform.position).normalized;
        direction.x *= flip;

        Vector3 originalLocalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float motion = Mathf.Sin(t * Mathf.PI);
            transform.localPosition = originalLocalPos + direction.normalized * motion * attackReach;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        if (!returnToStart) { rotateWeapon = true; } else
        {
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            transform.localScale = originalLocalScale;
        }
    }

}
