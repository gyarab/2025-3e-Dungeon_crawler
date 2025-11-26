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

        BoxCollider2D collider = hitbox.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(hitboxRange, hitboxWidth);
        collider.isTrigger = true;

        Damage damageComp = hitbox.AddComponent<Damage>();
        damageComp.SetDamage(damage);
        damageComp.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(RotateSword(attackDuration));
        return true;
    }


    IEnumerator RotateSword(float duration)
    {
        float elapsed = 0f; 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        int flip = mousePos.x < transform.parent.position.x ? -1 : 1; 
        Vector3 offset = new Vector3(attackOffset, 0f, 0f);
        transform.localScale = new Vector3(originalLocalScale.x * flip, originalLocalScale.y*flip, originalLocalScale.z);

        float startAngle = getMouseAngle() * flip + attackAngle+10f;
        float endAngle = getMouseAngle() * flip - attackAngle-10f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            transform.localPosition = Quaternion.Euler(0, 0, angle) * offset*flip;
            transform.localRotation = Quaternion.Euler(0, 0, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (!rotateWeapon) 
        { 
            transform.localPosition = originalLocalPos;
            transform.localRotation = originalLocalRot;
            transform.localScale = originalLocalScale;
        }
        if (cancelCooldownOnReturn)
        {
            CancelCooldown();
        }
    }

}
