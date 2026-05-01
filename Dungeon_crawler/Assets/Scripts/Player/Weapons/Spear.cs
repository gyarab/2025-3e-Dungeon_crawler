using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spear : Weapon
{
    [SerializeField] private bool cancelCooldownOnReturn = false;
    [SerializeField] private int damage = 25;
    [SerializeField] private int knockbackForce = 5;

    [SerializeField] private float hitboxRange = 2f;
    [SerializeField] private float hitboxWidth = 0.5f;

    [SerializeField] private float attackReach = 4f;
    [SerializeField] private float attackDuration = 0.4f;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    public override bool OnAttack()
    {
        if (!base.OnAttack()) return false;

        //rotates the weapon to face the mouse cursor
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (flip == -1) angle += 180f;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
        //creates hitbox
        GameObject hitbox = new GameObject("weaponHitbox");
        hitbox.transform.parent = transform;
        hitbox.transform.localPosition = Vector3.zero;
        hitbox.transform.localRotation = Quaternion.identity;
        BoxCollider2D col = hitbox.AddComponent<BoxCollider2D>();
        col.size = new Vector2(hitboxRange, hitboxWidth);
        col.isTrigger = true;
        //sets damage and knockback
        Damage dmg = hitbox.AddComponent<Damage>();
        dmg.SetDamage(damage);
        dmg.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(PikeSpear(attackDuration));
        return true;
    }

    IEnumerator PikeSpear(float duration)
    {
        //animates the spear thrusting forward and then returning to original position
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;

        Vector3 meshOriginalScale = weaponMesh.localScale;
        weaponMesh.localScale = new Vector3(flip, flip, 1);
        //rotates the weapon to face the mouse cursor
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldDir = (mousePos - transform.position).normalized;
        Vector3 localDir = transform.parent.InverseTransformDirection(worldDir).normalized;

        float elapsed = 0f;
        Vector3 start = transform.localPosition;
        //animates the spear thrusting forward and then returning to original position
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float m = Mathf.Sin(t * Mathf.PI);
            transform.localPosition = start + localDir * (m * attackReach);
            elapsed += Time.deltaTime;
            yield return null;
        }
        //resets position and rotation
        transform.localPosition = originalLocalPos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalLocalScale;
        weaponMesh.localScale = meshOriginalScale;

        if (cancelCooldownOnReturn) CancelCooldown();
        AttackFinished();
    }
}
