using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool destroyOnDeath = true;
    //[SerializeField] private GameObject healthBar;
    [SerializeField] private int immunityFrames = 60;

    //events
    [HideInInspector]
    public UnityEvent<int> HealthChanged;

    private bool hittable = true;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetHealth()
    {
        return currentHealth;
    }

    public bool TakeDamage(int damage)
    {
        if (!hittable) return false;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if(destroyOnDeath)
                Die();
            else
            {
                //do sth
            }
        }
        else
        {
            StartCoroutine(Immunity());
        }
        HealthChanged?.Invoke(currentHealth);
        return true;
    }

    public bool Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth);
        return true;
    }

    private IEnumerator Immunity()
    {
        hittable = false;
        yield return new WaitForSeconds(immunityFrames / 60f);
        hittable = true;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
