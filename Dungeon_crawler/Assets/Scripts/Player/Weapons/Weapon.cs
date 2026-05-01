using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum WeaponType
{
    Sword,
    Axe,
    Hammer,
    Spear,
    Daggers,
    ShortRange,
    Mace,
    Bomb
}

public class Weapon : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Sprite icon;
    public WeaponType type;
    [SerializeField] protected bool rotateMesh = false;
    [SerializeField] protected Transform weaponMesh;
    [SerializeField] protected float attackCooldown = 0.4f;
    [SerializeField] private Renderer handsRenderer;

    // determines whether this weapon flips with mouse / player direction
    [SerializeField] protected bool allowFlip = true;

    private SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];

    public bool canAttack = true;
    [HideInInspector] public UnityEvent attackEnd;

    protected int flip = 1;
    private Vector3 originalLocalSc;

    private bool followMouseDuringAttack = false;

    private void Awake()
    {
        //gets references and initializes variables
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
        handsRenderer = transform.parent.Find("Hands").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //subscribes to events and initializes sprite renderers
        transform.parent.GetComponent<PlayerMovement>().flipped.AddListener(Flip);
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in spriteRenderers)
        {
            sr.enabled = false;
        }
    }

    private void Update()
    {
        //if the player is holding the attack button, the weapon will follow the mouse and rotate towards it
        if (followMouseDuringAttack && allowFlip)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            int mouseFlip = mousePos.x > transform.parent.position.x ? 1 : -1;

            transform.parent.GetComponent<PlayerMovement>().Flip(mouseFlip);
            ApplyFlip(mouseFlip);
        }

        //rotates the weapon towards the mouse if rotateMesh is true
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
        //if the weapon cant attack or if there is no weapon equipped, return false
        if (!canAttack || WeaponManager.Instance.currentWeapon == null) return false;

        handsRenderer.enabled = false;
        if (spriteRenderers.Length > 0)
        {
            foreach (var sr in spriteRenderers)
            {
                sr.enabled = true;
            }
        }

        followMouseDuringAttack = true;

        //flips the weapon based on the mouse position relative to the player
        if (allowFlip)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            int mouseFlip = mousePos.x > transform.parent.position.x ? 1 : -1;

            transform.parent.GetComponent<PlayerMovement>().Flip(mouseFlip);
            ApplyFlip(mouseFlip);
        }

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
        //resets variables and invokes the attack end event
        handsRenderer.enabled = true;

        foreach (var sr in spriteRenderers)
        {
            sr.enabled = false;
        }

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
        if (!allowFlip)
            return;

        //if the weapon is currently attacking, wait until the attack is finished to apply the flip
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
        //applies the pending flip and removes the listener to prevent it from being called multiple times
        attackEnd.RemoveListener(ApplyPendingFlip);
        ApplyFlip(pendingFlip);
    }

    private void ApplyFlip(int flip)
    {
        //applies the flip to the weapon and weapon mesh
        if (!allowFlip)
            return;

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