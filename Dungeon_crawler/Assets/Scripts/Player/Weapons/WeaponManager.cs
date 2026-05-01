using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class WeaponManager : MonoBehaviour
{
    //keeps track of player weapons and equips them
    public static WeaponManager Instance;
    public GameObject currentWeapon;
    public List<GameObject> weapons = new List<GameObject>();
    
    //private int currentWeaponIndex = 0;
    private Weapon weaponScript;
    private PlayerInput playerInput;
    private InputAction attackAction;

    private int pendingIndex = 0;

    private void Awake()
    {
        //initializes singleton and sets up input actions
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];

        attackAction.performed += OnAttack;
        attackAction.canceled += OnAttackReleased;
        if (Instance == null)
        {   
            Instance = this;
        }
    }

    public void OnWeaponKey(InputAction.CallbackContext ctx)
    {
        //handles weapon switching input
        //checks if the key pressed is a number key, switches to the corresponding weapon in the list
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
        //switches to the weapon at the given index in the list
        if (index < 1 || index > weapons.Count) return;
        if (weaponScript !=null && !weaponScript.canAttack)
        {
            pendingIndex = index;
            weaponScript.attackEnd.AddListener(OnCooldownFinished);
            return;
        }
        Destroy(currentWeapon);
        currentWeapon = Instantiate(weapons[index - 1], transform);
        weaponScript = currentWeapon.GetComponent<Weapon>();
        weaponScript.Flip(transform.GetComponent<PlayerMovement>().flip);
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
