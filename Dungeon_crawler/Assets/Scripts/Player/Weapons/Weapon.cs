using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Sprite icon;
    [SerializeField] protected bool rotateMesh = false;
    [SerializeField] protected Transform weaponMesh;
    [SerializeField] protected float attackCooldown = 0.4f;

    public bool canAttack = true;
    [HideInInspector] public UnityEvent attackEnd;

    protected int flip = 1;
    private Vector3 originalLocalSc;

    private bool followMouseDuringAttack = false;

    private void Awake()
    {
        playerInput = transform.parent.GetComponent<PlayerInput>();
        originalLocalSc = transform.localScale;
        if (weaponMesh == null)
        {
            try
            {
                weaponMesh = transform.Find("Mesh");
            }
            catch
            {
                Debug.LogWarning("Weapon mesh not assigned and 'Mesh' child not found.");
            }
        }
    }

    private void Start()
    {
        transform.parent.GetComponent<PlayerMovement>().flipped.AddListener(Flip);
    }

    private void Update()
    {
        if (followMouseDuringAttack)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            int mouseFlip = mousePos.x > transform.parent.position.x ? 1 : -1;

            transform.parent.GetComponent<PlayerMovement>().Flip(mouseFlip);

            ApplyFlip(mouseFlip);
        }

        if (rotateMesh && weaponMesh != null)
        {
            weaponMesh.rotation = Quaternion.Euler(0, 0, getMouseAngle());

            weaponMesh.localScale = new Vector3(
                originalLocalSc.x,
                originalLocalSc.y,
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

    public virtual void OnAttackReleased()
    {
        followMouseDuringAttack = false;
    }

    public virtual bool OnAttack()
    {
        if (!canAttack) return false;

        followMouseDuringAttack = true;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        int mouseFlip = mousePos.x > transform.parent.position.x ? 1 : -1;

        transform.parent.GetComponent<PlayerMovement>().Flip(mouseFlip);

        ApplyFlip(mouseFlip);

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
        followMouseDuringAttack = false;

        canAttack = true;
        attackEnd.Invoke();
    }

    public void CancelCooldown()
    {
        AttackFinished();
    }

    private int pendingFlip;

    public void Flip(int flip)
    {
        if (!canAttack)
        {
            pendingFlip = flip;
            attackEnd.AddListener(ApplyPendingFlip);
            return;
        }

        ApplyFlip(flip);
    }

    private void ApplyPendingFlip()
    {
        attackEnd.RemoveListener(ApplyPendingFlip);
        ApplyFlip(pendingFlip);
    }

    private void ApplyFlip(int flip)
    {
        this.flip = flip;

        transform.localScale = new Vector3(flip, 1, 1);

        if (weaponMesh != null)
        {
            weaponMesh.localScale = new Vector3(
                originalLocalSc.x,
                originalLocalSc.y,
                originalLocalSc.z
            );
        }
    }
}
