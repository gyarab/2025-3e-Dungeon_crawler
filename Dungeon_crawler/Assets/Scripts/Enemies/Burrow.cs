using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Burrow : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 0.3f;
    public float jumpSpeed = 3f;

    [Header("Burrow Settings")]
    public float burrowSpeed = 2f;
    public ParticleSystem buryEffect;
    public GameObject maskPrefab;

    private Vector3 originalPos;
    private GameObject mask;
    private Health health;
    private Damage damage;
    private BoxCollider2D boxCollider;
    private GameObject enemy;
    private float enemySizeX;
    private float enemySizeY;

    public UnityEvent<bool> isBurried;

    private void Start()
    {
        health = GetComponent<Health>();
        damage = GetComponent<Damage>();
        boxCollider = GetComponent<BoxCollider2D>();

        enemySizeX = boxCollider.size.x * transform.localScale.x;
        enemySizeY = boxCollider.size.y * transform.localScale.y;

        enemy = transform.Find("enemy").gameObject;
    }

    public void Bury()
    {
        originalPos = enemy.transform.position;

        if (buryEffect != null)
            buryEffect.Play();

        if (maskPrefab != null)
        {
            mask = Instantiate(maskPrefab,
                new Vector3(transform.position.x, transform.position.y - enemySizeY * 1.5f, 0),
                Quaternion.identity);

            mask.transform.localScale = new Vector3(enemySizeX * 1.5f, enemySizeY * 1.5f, 1);
            mask.transform.parent = transform;

            RefreshSpritesForMasking();
        }

        health.enabled = false;
        damage.enabled = false;
        boxCollider.enabled = false;

        isBurried?.Invoke(true);

        StartCoroutine(BurrowSequence());
    }

    private IEnumerator BurrowSequence()
    {
        Vector3 jumpTarget = originalPos + Vector3.up * jumpHeight;
        //jump up
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * jumpSpeed;
            enemy.transform.position = Vector3.Lerp(originalPos, jumpTarget, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        Vector3 dropTarget = mask.transform.position;
        //drop down
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * jumpSpeed;
            enemy.transform.position = Vector3.Lerp(jumpTarget, dropTarget, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        RefreshSpritesForMasking();
    }

    private void RefreshSpritesForMasking()
    {
        var renderers = enemy.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in renderers)
        {
            var s = sr.sprite;
            sr.sprite = null;
            sr.sprite = s;
        }
    }
}