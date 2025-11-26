using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public GameObject currentWeapon;
    private Weapon weaponScript;
    private PlayerInput playerInput;
    private InputAction attackAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];

        attackAction.performed += OnAttack;
        attackAction.canceled += OnAttackReleased;
    }

    private void Start()
    {
        if (currentWeapon != null)
            weaponScript = currentWeapon.GetComponent<Weapon>();
    }

    public void ChangeWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        weaponScript = currentWeapon.GetComponent<Weapon>();
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
