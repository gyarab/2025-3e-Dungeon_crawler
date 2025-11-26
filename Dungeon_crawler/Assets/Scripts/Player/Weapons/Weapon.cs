using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] protected bool rotateWeapon = false;
    protected bool canAttack = true;
    [SerializeField] protected float attackCooldown = 0.4f;

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

            transform.localScale = new Vector3(flip, Mathf.Abs(transform.localScale.y), 1);
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
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    public void CancelCooldown()
    {
        StopAllCoroutines();
        canAttack = true;
    }
}
