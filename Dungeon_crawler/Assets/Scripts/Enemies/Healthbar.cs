using UnityEngine;

public class Healthbar : MonoBehaviour
{
    //[SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject target;
    private Health health;
    private Transform healthbarBG;
    private Transform healthbarBar;
    private float initialBarScaleX;

    private void Start()
    {
        if (target == null)
        {
            target = transform.parent.gameObject;
        }
        health = target.transform.GetComponent<Health>();
        health.HealthChanged.AddListener(ChangeHealthbar);
        healthbarBG = transform.GetChild(0).gameObject.transform;
        healthbarBar = transform.GetChild(1).gameObject.transform; 
        healthbarBar = transform.GetChild(1).gameObject.transform;
        initialBarScaleX = healthbarBar.localScale.x;

    }

    private void ChangeHealthbar(int hp)
    {
        if (hp < 0)
        {
            hp = 0;
        }
        float maxHealth = health.GetMaxHealth();
        float percent = Mathf.Clamp01((float)hp / maxHealth);

        healthbarBar.localScale = new Vector3(initialBarScaleX * percent, healthbarBar.localScale.y, healthbarBar.localScale.z);
        healthbarBar.localPosition = new Vector3(-((1 - percent) / 2) * initialBarScaleX, healthbarBar.localPosition.y, healthbarBar.localPosition.z);
    }


}
