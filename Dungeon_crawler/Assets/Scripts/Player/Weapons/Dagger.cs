using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dagger : Weapon
{
    [SerializeField] private bool cancelCooldownOnReturn = false;
    [SerializeField] private int damage = 25;
    [SerializeField] private int knockbackForce = 5;

    [SerializeField] private float hitboxRange = 2f;
    [SerializeField] private float hitboxWidth = 0.5f;

    [SerializeField] private float attackReach = 4f;
    [SerializeField] private float attackDuration = 0.4f;

    [SerializeField] private GameObject[] weaponSprites;
    private int weaponIndex;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    public override bool OnAttack()
    {
        if (!base.OnAttack()) return false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (flip == -1) angle += 180f;
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        GameObject dagger = weaponSprites[weaponIndex];
        weaponIndex = (weaponIndex + 1) % weaponSprites.Length;

        GameObject hitbox = new GameObject("weaponHitbox");
        hitbox.transform.parent = dagger.transform;
        hitbox.transform.localPosition = Vector3.zero;
        hitbox.transform.localRotation = Quaternion.identity;
        BoxCollider2D col = hitbox.AddComponent<BoxCollider2D>();
        col.size = new Vector2(hitboxRange, hitboxWidth);
        col.isTrigger = true;

        Damage dmg = hitbox.AddComponent<Damage>();
        dmg.SetDamage(damage);
        dmg.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(PikeDagger(dagger, attackDuration));
        return true;
    }

    IEnumerator PikeDagger(GameObject dagger, float duration)
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;

        Vector3 dPos = dagger.transform.localPosition;
        Quaternion dRot = dagger.transform.localRotation;
        Vector3 dScale = dagger.transform.localScale;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldDir = (mousePos - transform.position).normalized;
        Vector3 localDir = transform.InverseTransformDirection(worldDir).normalized * flip;

        float elapsed = 0f;
        Vector3 start = dagger.transform.localPosition;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float m = Mathf.Sin(t * Mathf.PI);
            dagger.transform.localPosition = start + localDir * (m * attackReach);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalLocalScale;

        dagger.transform.localPosition = dPos;
        dagger.transform.localRotation = dRot;
        dagger.transform.localScale = dScale;

        if (cancelCooldownOnReturn) CancelCooldown();
        AttackFinished();
    }
}
