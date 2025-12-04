using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class WeaponManager : MonoBehaviour
{
    public GameObject currentWeapon;
    public List<GameObject> weapons;
    private int currentWeaponIndex = 0;
    private Weapon weaponScript;
    private PlayerInput playerInput;
    private InputAction attackAction;

    private int pendingIndex = 0;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];

        attackAction.performed += OnAttack;
        attackAction.canceled += OnAttackReleased;

        transform.GetComponent<PlayerMovement>().flipped.AddListener(FlipWeapon);
    }

    private void Start()
    {
        currentWeapon?.SetActive(false);
        foreach(GameObject weapon in weapons)
        {
            weapon?.SetActive(false);
        }
    }
    private void FlipWeapon(bool isFlipped)
    {
        Vector3 scale = currentWeapon.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFlipped ? -1 : 1);
        currentWeapon.transform.localScale = scale;
    }

    public void OnWeaponKey(InputAction.CallbackContext ctx)
    {
        KeyControl keyControl = ctx.control as KeyControl;

        string keyName = keyControl.keyCode.ToString();

        if (int.TryParse(keyName.Replace("Digit", ""), out int weaponIndex))
        {
            if (weaponIndex < 1 || weaponIndex > weapons.Count)
            {
                return;
            }
            ChangeWeapon(weaponIndex);
        }
    }

    public void ChangeWeapon(int index)
    {
        if (index < 1 || index > weapons.Count) return;
        if (weaponScript !=null && !weaponScript.canAttack)
        {
            pendingIndex = index;
            weaponScript.attackEnd.AddListener(OnCooldownFinished);
            return;
        }
        currentWeapon.SetActive(false);
        currentWeapon = weapons[index - 1];
        currentWeapon.SetActive(true);
        weaponScript = currentWeapon.GetComponent<Weapon>();
    }

    private void OnCooldownFinished()
    {
        weaponScript.attackEnd.RemoveListener(OnCooldownFinished);
        ChangeWeapon(pendingIndex);
        pendingIndex = -1;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        weaponScript?.OnAttack();
    }

    private void OnAttackReleased(InputAction.CallbackContext context)
    {
        weaponScript?.OnAttackReleased();
    }
}
