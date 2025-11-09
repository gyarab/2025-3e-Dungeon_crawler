using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField]protected bool rotateWeapon = false;
    protected bool canAttack = true;
    [SerializeField] protected float attackCooldown = 0.4f;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if(rotateWeapon)
        {
            transform.rotation = Quaternion.Euler(0, 0, getMouseAngle());
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int flip = mousePos.x < transform.position.x ? -1 : 1;
            transform.localScale = new Vector3(1 * flip, 1, 1);
        }
    }
    public float getMouseAngle()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    public virtual bool OnAttack()
    {
        if(!canAttack) return false;
        StartCoroutine(AttackCooldown(attackCooldown));
        return true;
        //attack overriden in subclasses
    }

    IEnumerator AttackCooldown(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

}
