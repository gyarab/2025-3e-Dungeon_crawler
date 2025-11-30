using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Basic settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int immunityFrames = 60;
    [Header("Effects")]
    [SerializeField] private float shakeOnHitIntensity = 1f;
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private Color particleColor = Color.red;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private bool transformToOnDeath = false;
    [SerializeField] private GameObject[] transformTo;

    private Transform parentTransform;

    [HideInInspector] public UnityEvent<int> HealthChanged;

    private bool hittable = true;

    private void Start()
    {
        currentHealth = maxHealth; 
        parentTransform = gameObject.transform.parent;
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
            if (deathParticles != null)
            {
                ParticleSystem ps = Instantiate(deathParticles, transform.position, Quaternion.identity);

                var main = ps.main;
                main.startColor = particleColor;
                ps.transform.position = new Vector3(transform.position.x, transform.position.y, parentTransform.position.z + 0.1f);

                ps.Play();
                Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
            }

            if (transformToOnDeath && transformTo.Length > 0)
                TransformToOnDeath();
            if (destroyOnDeath)
                Die();
            return false;
        }

        StartCoroutine(Immunity());
        StartCoroutine(Shake());

        if (hitParticles != null)
        {
            ParticleSystem ps = Instantiate(hitParticles, transform.position, Quaternion.identity);

            var main = ps.main;
            main.startColor = particleColor;
            ps.transform.position = new Vector3(transform.position.x, transform.position.y, parentTransform.position.z+0.1f);

            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
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

    private IEnumerator Shake()
    {
        Vector3 originalPos = parentTransform.localPosition;
        float t = 0f;

        while (t < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeOnHitIntensity;
            float y = Random.Range(-1f, 1f) * shakeOnHitIntensity;
            parentTransform.localPosition = originalPos + new Vector3(x, y, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        parentTransform.localPosition = originalPos;
    }

    private void Die()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
    private void TransformToOnDeath()
    {
        Random.Range(0, transformTo.Length);
        Instantiate(transformTo[Random.Range(0, transformTo.Length)], gameObject.transform.parent.position, Quaternion.identity);
    }
}
