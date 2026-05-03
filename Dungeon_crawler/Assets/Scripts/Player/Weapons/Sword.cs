using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : Weapon
{
    [SerializeField] private bool cancelCooldownOnReturn = false;

    [Header("Damage")]
    [SerializeField] private int damage = 25;
    [SerializeField] private int knockbackForce = 5;

    [Header("Hitbox")]
    [SerializeField] private float hitboxRange = 1.5f;
    [SerializeField] private float hitboxWidth = 0.5f;

    [Header("Attack Motion")]
    [SerializeField] private float attackOffset = 1.3f;
    [SerializeField] private float attackAngle = 45f;
    [SerializeField] private float attackDuration = 0.2f;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    public override bool OnAttack()
    {
        if (!base.OnAttack()) { return false; }

        //creates hitbox as a child of the weapon, with the same position and rotation, and destroys it after the attack duration
        GameObject hitbox = new GameObject("weaponHitbox");
        hitbox.transform.SetParent(transform, false);
        hitbox.transform.localPosition = new Vector3(hitboxRange * 0.5f, 0f, 0f);
        hitbox.transform.localRotation = Quaternion.identity;

        //the hitbox is a box collider with the size of the hitbox range and width, and is a trigger
        BoxCollider2D collider = hitbox.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(hitboxRange, hitboxWidth);
        collider.offset = Vector2.zero;
        collider.isTrigger = true;

        //the hitbox has a damage component with the damage and knockback force of the weapon, and is destroyed after the attack duration
        Damage damageComp = hitbox.AddComponent<Damage>();
        damageComp.SetDamage(damage);
        damageComp.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(RotateSword(attackDuration));
        return true;
    }

    IEnumerator RotateSword(float duration)
    {
        //stores the original local position, rotation, and scale of the weapon to reset it after the attack
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;

        //flips the weapon based on the direction of the mouse
        Vector3 originalLocalScW = weaponMesh.localScale;
        Vector3 originalLocalRW = weaponMesh.localRotation.eulerAngles;

        float elapsed = 0f;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 origin = transform.parent.position;

        bool facingLeft = mouseWorldPos.x < origin.x;

        weaponMesh.localScale = Vector3.one;
        weaponMesh.localRotation = Quaternion.identity;
        weaponMesh.localPosition = new Vector3(attackOffset, 0f, 0f);

        float baseAngle = Mathf.Atan2(
            mouseWorldPos.y - origin.y,
            mouseWorldPos.x - origin.x
        ) * Mathf.Rad2Deg;

        if (facingLeft)
            baseAngle += 180f;

        //calculates the start and end angles
        float startAngle = baseAngle + attackAngle;
        float endAngle = baseAngle - attackAngle;

        if(facingLeft)
        {
            float temp = startAngle;
            startAngle = endAngle;
            endAngle = temp;
        }

        //rotates the weapon
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float smooth = Mathf.Sin(t * Mathf.PI * 0.5f);

            float angle = Mathf.LerpAngle(startAngle, endAngle, smooth);

            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        //resets the weapon to its original position, rotation, and scale
        //rotateMesh is obsolete, as the weapon is no longer visible when not attacking
        if (!rotateMesh)
        {
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            transform.localScale = originalLocalScale;
        }

        weaponMesh.localScale = originalLocalScW;
        weaponMesh.localRotation = Quaternion.Euler(originalLocalRW);

        if (cancelCooldownOnReturn) CancelCooldown();
        AttackFinished();
    }
}