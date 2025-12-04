using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Sprite icon;
    [SerializeField] protected bool rotateWeapon = false;
    [Header("Cooldown >= duration !")]
    [SerializeField] protected float attackCooldown = 0.4f;

    public bool canAttack = true;
    [HideInInspector] public UnityEvent attackEnd;

    public bool isFlipped;
    protected int flip = 1;
    private Vector3 originalLocalSc;

    private void Awake()
    {
        playerInput = transform.parent.GetComponent<PlayerInput>();
        originalLocalSc = transform.localScale;
    }

    private void Start()
    {
        transform.parent.GetComponent<PlayerMovement>().flipped.AddListener(Flip);
    }

    private void Update()
    {
        if (rotateWeapon)
        {
            transform.rotation = Quaternion.Euler(0, 0, getMouseAngle());

            int flip = isFlipped ? -1 : 1;
            transform.localScale = new Vector3(
                originalLocalSc.x,
                originalLocalSc.y * flip,
                originalLocalSc.z
            );
        }
    }

    public float getMouseAngle()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public virtual void OnAttackReleased() { }

    public virtual bool OnAttack()
    {
        if (!canAttack) return false;

        StartCoroutine(AttackCooldown(attackCooldown));
        return true;
    }

    IEnumerator AttackCooldown(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time + 0.1f);
    }

    public void AttackFinished()
    {
        canAttack = true;
        attackEnd.Invoke();
    }

    public void CancelCooldown()
    {
        AttackFinished();
    }

    private bool pendingFlip;

    public void Flip(bool newFlip)
    {
        if (!canAttack)
        {
            pendingFlip = newFlip;
            attackEnd.AddListener(ApplyPendingFlip);
            return;
        }

        ApplyFlip(newFlip);
    }

    private void ApplyPendingFlip()
    {
        attackEnd.RemoveListener(ApplyPendingFlip);
        ApplyFlip(pendingFlip);
    }

    private void ApplyFlip(bool newFlip)
    {
        isFlipped = newFlip;

        flip = isFlipped ? -1 : 1;

        transform.localScale = new Vector3(
            originalLocalSc.x * flip,
            originalLocalSc.y,
            originalLocalSc.z
        );
    }
}
