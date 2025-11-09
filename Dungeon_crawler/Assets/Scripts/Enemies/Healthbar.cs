using UnityEngine;

public class Healthbar : MonoBehaviour
{
    //[SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject target;
    private Health health;
    private Transform healthbarBG;
    private Transform healthbarBar;

    private void Start()
    {
        if (target == null)
        {
            target = transform.parent.gameObject;
        }
        health = target.GetComponent<Health>();
        health.HealthChanged.AddListener(ChangeHealthbar);
        healthbarBG = transform.GetChild(0).gameObject.transform;
        healthbarBar = transform.GetChild(1).gameObject.transform;
    }

    private void ChangeHealthbar(int hp)
    {
        healthbarBar.transform.localScale = new Vector3((float)hp / health.GetMaxHealth(), healthbarBG.localScale.y, healthbarBG.localScale.z);
        healthbarBar.transform.localPosition = new Vector3(-((1 - (float)hp / health.GetMaxHealth()) / 2) * healthbarBG.localScale.x, healthbarBar.transform.localPosition.y, healthbarBar.transform.localPosition.z);
    }
}
