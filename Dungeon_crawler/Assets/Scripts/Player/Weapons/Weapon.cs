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

    private InputAction attackAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }


    private void Update()
    {
        if (rotateWeapon)
        {
            transform.rotation = Quaternion.Euler(0, 0, getMouseAngle());
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int flip = mousePos.x < transform.position.x ? -1 : 1;

            transform.localScale = new Vector3(flip, flip, 1);
        }
    }

    public float getMouseAngle()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }


    public virtual bool OnAttack()
    {
        if (!canAttack) return false;

        StartCoroutine(AttackCooldown(attackCooldown));

        return true;
    }

    public virtual void OnAttackReleased()
    {
    }

    IEnumerator AttackCooldown(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time+0.1f);
    }

    public void AttackFinished()
    {
        canAttack = true;
        attackEnd.Invoke();
    }

    public void CancelCooldown()
    {
        StopAllCoroutines();
        canAttack = true;
    }
}
