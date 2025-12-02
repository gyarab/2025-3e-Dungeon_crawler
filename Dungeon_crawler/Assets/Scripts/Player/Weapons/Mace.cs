using UnityEngine;
using System.Collections;

public class Mace : Weapon
{
    [SerializeField] private bool cancelCooldownOnReturn = true;
    [Header("Damage")]
    [SerializeField] private int baseDamage = 25;
    [SerializeField] private float maxDamageMultiplier = 3f;
    [SerializeField] private float hitboxRange = 1.5f;
    [SerializeField] private float hitboxWidth = 0.5f;
    [SerializeField] private int knockbackForce = 5;

    [Header("Spinning")]
    [SerializeField] private float spinRadius = 1.5f;
    [SerializeField] private float minSpinSpeed = 180f; 
    [SerializeField] private float maxSpinSpeed = 720f; 
    [SerializeField] private float spinAcceleration = 400f;

    [Header("Throwing")]
    [SerializeField] private float baseSpeed = 12f;
    [SerializeField] private float maxSpeedMultiplier = 3f;
    [SerializeField] private float maceMaxRange = 3f;
    [SerializeField] private float maxRangeMultiplier = 3f; 

    private GameObject hitbox;
    private bool isSpinning = false;
    private float angle = 0f;
    private float currentSpinSpeed;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    private float spinDuration = 0f; 

    void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        originalLocalScale = transform.localScale;
    }

    public override bool OnAttack()
    {
        if (!base.OnAttack()) { return false; }

        StartSpinning();
        return true;
    }

    public override void OnAttackReleased()
    {
        if (isSpinning)
            StartCoroutine(ThrowMace());
    }

    private void StartSpinning()
    {
        if (isSpinning) return;
        isSpinning = true;
        spinDuration = 0f;
        currentSpinSpeed = minSpinSpeed;
        StartCoroutine(SpinRoutine());

        hitbox = new GameObject("MaceHitbox");
        hitbox.transform.parent = transform;
        hitbox.transform.localPosition = Vector3.zero;
        var col = hitbox.AddComponent<BoxCollider2D>();
        col.size = new Vector2(hitboxRange, hitboxWidth);
        col.isTrigger = true;

        var dmg = hitbox.AddComponent<Damage>();
        dmg.SetDamage(baseDamage);
        dmg.SetKnockbackForce(knockbackForce);
    }

    private IEnumerator SpinRoutine()
    {
        angle = 0f;

        while (isSpinning)
        {
            spinDuration += Time.deltaTime;

            currentSpinSpeed += spinAcceleration * Time.deltaTime;
            currentSpinSpeed = Mathf.Min(currentSpinSpeed, maxSpinSpeed);

            angle += currentSpinSpeed * Time.deltaTime;
            float rad = angle * Mathf.Deg2Rad;

            transform.localPosition = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * spinRadius;

            float angleToCenter = Mathf.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0, 0, angleToCenter + 90);

            yield return null;
        }
    }

    private IEnumerator ThrowMace()
    {
        isSpinning = false;

        Vector3 playerPos = transform.parent.position;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 dir = (mouseWorld - playerPos).normalized;

        float rangeMultiplier = Mathf.Min(1 + spinDuration, maxRangeMultiplier);
        float speedMultiplier = Mathf.Min(1 + spinDuration, maxSpeedMultiplier);
        float damageMultiplier = Mathf.Min(1 + spinDuration, maxDamageMultiplier);
        float throwRange = maceMaxRange * rangeMultiplier;

        int throwDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        hitbox?.GetComponent<Damage>().SetDamage(throwDamage);

        float traveled = 0f;
        Vector3 startPos = playerPos;

        while (traveled < throwRange)
        {
            float step = baseSpeed * speedMultiplier * Time.deltaTime;
            traveled += step;

            transform.position = startPos + dir * traveled;

            yield return null;
        }
        //return
        traveled = 0f;
        while (traveled < throwRange)
        {
            Vector3 targetPos = transform.parent.position; 
            Vector3 dirToPlayer = (targetPos - transform.position).normalized;

            float step = baseSpeed * speedMultiplier * Time.deltaTime;
            traveled += step;

            transform.position += dirToPlayer * step;

            yield return null;
        }


        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        transform.localScale = originalLocalScale;

        Destroy(hitbox);
        if (cancelCooldownOnReturn)
        {
            CancelCooldown();
        }
        AttackFinished();
    }
}
