using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : Weapon
{
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackOffset = 0.5f;
    [SerializeField] private float hitboxRange = 1.5f;
    [SerializeField] private float hitboxWidth = 0.5f;
    [SerializeField] private float attackAngle = 45f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private int knockbackForce = 5;
    private bool canAttack = true;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;
    }


    public override void OnAttack()
    {
        if (!canAttack) return;
        StartCoroutine(AttackCooldown());

        GameObject hitbox = new GameObject("SwordHitbox");
        hitbox.transform.parent = transform;
        hitbox.transform.localRotation = Quaternion.identity;
        hitbox.transform.position = transform.position;
        //hitbox.transform.localPosition = Vector3.right;
        //(hitboxRange / 2) *
        BoxCollider2D collider = hitbox.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(hitboxRange, hitboxWidth);
        collider.isTrigger = true;

        Damage swordHitbox = hitbox.AddComponent<Damage>();
        swordHitbox.SetDamage(damage);
        swordHitbox.SetKnockbackForce(knockbackForce);

        Destroy(hitbox, attackDuration);

        StartCoroutine(RotateSword(attackDuration));
    }


    IEnumerator RotateSword(float duration)
    {
        float elapsed = 0f; 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        int flip = mousePos.x < transform.parent.position.x ? -1 : 1; 
        Vector3 offset = new Vector3(attackOffset, 0f, 0f);
        transform.localScale = new Vector3(originalLocalScale.x * flip, originalLocalScale.y, originalLocalScale.z);

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

        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        transform.localScale = originalLocalScale;
    }





    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

}
