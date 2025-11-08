using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public GameObject currentWeapon;
    private Weapon weaponScript;

    private void Start()
    {
        weaponScript = currentWeapon.GetComponent<Weapon>();
    }
    public void ChangeWeapon(GameObject weapon) {
        currentWeapon = weapon;
        weaponScript = currentWeapon.GetComponent<Weapon>();
    }

    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("WeaponManager OnAttack");
        weaponScript?.OnAttack();
    }
    
}
