using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dagger : Weapon
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

    [Header("Visuals")]
    [SerializeField] private GameObject[] weaponSprites;
    private int weaponIndex;

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

        GameObject hitbox = new GameObject("weaponHitbox");
        hitbox.transform.parent = transform;
        hitbox.transform.localRotation = Quaternion.identity;
        hitbox.transform.position = transform.position;
        BoxCollider2D damageComp = hitbox.AddComponent<BoxCollider2D>();
        damageComp.size = new Vector2(hitboxRange, hitboxWidth);
        damageComp.isTrigger = true;

        Damage weaponHitbox = hitbox.AddComponent<Damage>();
        weaponHitbox.SetDamage(damage);
        weaponHitbox.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        GameObject currentDagger = weaponSprites[weaponIndex];

        weaponIndex = (weaponIndex + 1) % weaponSprites.Length;

        hitbox.transform.parent = currentDagger.transform;
        StartCoroutine(PikeDagger(currentDagger, attackDuration));
        return true;
    }

    IEnumerator PikeDagger(GameObject dagger, float duration)
    {
        Vector3 originalLocalPosDagger = dagger.transform.localPosition;
        Quaternion originalLocalRotDagger = dagger.transform.localRotation;
        Vector3 originalLocalScaleDagger = dagger.transform.localScale;

        bool returnToStart = !rotateWeapon;
        rotateWeapon = true;
        yield return new WaitForSeconds(0.05f);
        rotateWeapon = false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int flip = mousePos.x < transform.parent.position.x ? -1 : 1;

        Vector3 direction = Vector3.right;
        //Vector3 direction = (mousePos - transform.position).normalized;
        //direction.x *= flip;

        Vector3 startPos = dagger.transform.localPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float motion = Mathf.Sin(t * Mathf.PI);

            dagger.transform.localPosition = startPos + direction * motion * attackReach;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!returnToStart) { rotateWeapon = true; }
        else
        {
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            transform.localScale = originalLocalScale;
            dagger.transform.localPosition = originalLocalPosDagger;
            dagger.transform.localRotation = originalLocalRotDagger;
            dagger.transform.localScale = originalLocalScaleDagger;
        }
        if (cancelCooldownOnReturn)
        {
            CancelCooldown();
        }
    }

}
