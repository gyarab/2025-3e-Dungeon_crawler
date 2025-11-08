using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private PlayerInput playerInput;
    public bool rotateWeapon = false;

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

    public virtual void OnAttack()
    {
        //attack overriden in subclasses
    }

}
