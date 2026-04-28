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
    [SerializeField] private float delayDeath = 0f;
    [SerializeField] private string deathTriggerName = "Die";
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;

    public UnityEvent ActionsOnDeath;

    [HideInInspector] public UnityEvent<int> HealthChanged;

    private bool hittable = true;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void ChangeHealth(int value)
    {
        if (value > 0)
        {
            Heal(value);
        }
        else
        {
            TakeDamage(value);
        }
    }

    public bool TakeDamage(int damage)
    {
        if (!hittable || isDead) return false;

        currentHealth -= damage;

        HealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;

            if (transformToOnDeath && transformTo.Length > 0)
                TransformToOnDeath();

            StartCoroutine(Die());
            return false;
        }

        StartCoroutine(Immunity());
        StartCoroutine(Shake());

        if (hitParticles != null)
        {
            ParticleSystem ps = Instantiate(hitParticles, transform.position, Quaternion.identity);

            var main = ps.main;
            main.startColor = particleColor;
            ps.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);

            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        return true;
    }

    public bool Heal(int amount)
    {
        if (isDead) return false;

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
        Vector3 originalPos = transform.localPosition;
        float t = 0f;

        while (t < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeOnHitIntensity;
            float y = Random.Range(-1f, 1f) * shakeOnHitIntensity;
            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    private IEnumerator Die()
    {
        ActionsOnDeath?.Invoke();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }


        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        yield return new WaitForSeconds(delayDeath);

        if (deathParticles != null)
        {
            ParticleSystem ps = Instantiate(deathParticles, transform.position, Quaternion.identity);

            var main = ps.main;
            main.startColor = particleColor;
            ps.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);

            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    private void TransformToOnDeath()
    {
        Instantiate(transformTo[Random.Range(0, transformTo.Length)], transform.position, Quaternion.identity);
    }
}