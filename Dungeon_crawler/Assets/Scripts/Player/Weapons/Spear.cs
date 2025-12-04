using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spear : Weapon
{
    [SerializeField] private bool cancelCooldownOnReturn = false;
    [Header("Damage")]
    [SerializeField] private int damage = 25;
    [SerializeField] private int knockbackForce = 5;

    [Header("Hitbox")]
    [SerializeField] private float hitboxRange = 2f;
    [SerializeField] private float hitboxWidth = 0.5f;

    [Header("Attack Motion")]
    [SerializeField] private float attackReach = 4f;
    [SerializeField] private float attackDuration = 0.4f;
    //[SerializeField] private float attackOffset = 0.5f;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;


    public override bool OnAttack()
    {
        if (!base.OnAttack()) { return false; }

        GameObject hitbox = new GameObject("weaponHitbox");
        hitbox.transform.parent = transform;
        hitbox.transform.localRotation = Quaternion.identity;
        hitbox.transform.position = transform.position;
        BoxCollider2D collider = hitbox.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(hitboxRange, hitboxWidth);
        collider.isTrigger = true;

        Damage damageComp = hitbox.AddComponent<Damage>();
        damageComp.SetDamage(damage);
        damageComp.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(PikeSpear(attackDuration));

        return true;
    }


    IEnumerator PikeSpear(float duration)
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;

        bool returnToStart = !rotateWeapon;
        rotateWeapon = true;
        yield return new WaitForSeconds(0.05f);
        rotateWeapon = false;

        Vector3 startLocalPos = originalLocalPos;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldDir = (mousePos - transform.position).normalized;

        Vector3 localDir = transform.parent.InverseTransformDirection(worldDir).normalized;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float motion = Mathf.Sin(t * Mathf.PI);

            transform.localPosition = startLocalPos + localDir * (motion * attackReach);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startLocalPos;

        if (!returnToStart)
        {
            rotateWeapon = true;
        }
        else
        {
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            transform.localScale = originalLocalScale;
        }

        if (cancelCooldownOnReturn)
            CancelCooldown();

        AttackFinished();
    }



}
