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
    public GameObject buryEffect;
    public GameObject mask;
    public float burrowParticleYpos = 1.5f;

    private Vector3 originalPos;
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
        {
            GameObject buryEffectGO = Instantiate(buryEffect, new Vector3(transform.position.x, transform.position.y - enemySizeY / burrowParticleYpos, transform.position.z-1f), Quaternion.identity);
            buryEffectGO.transform.localScale = new Vector3(transform.localScale.x*1.5f, transform.localScale.y*1.5f, 0);
            buryEffectGO.GetComponent<ParticleSystem>().Play();
            Destroy(buryEffectGO, buryEffectGO.GetComponent<ParticleSystem>().main.duration + buryEffectGO.GetComponent<ParticleSystem>().main.startLifetime.constantMax);
        }
        mask.SetActive(true);

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

    public void Unbury()
    {
        if (mask == null)
            return;

        StartCoroutine(UnburrowSequence());
    }

    private IEnumerator UnburrowSequence()
    {
        Vector3 maskCenter = mask.transform.position;
        Vector3 riseTarget = maskCenter + Vector3.up * jumpHeight;

        if (buryEffect != null)
        {
            GameObject buryEffectGO = Instantiate(buryEffect, new Vector3(transform.position.x, transform.position.y - enemySizeY / burrowParticleYpos, transform.position.z - 1f), Quaternion.identity);
            buryEffectGO.transform.localScale = new Vector3(transform.localScale.x * 1.5f, transform.localScale.y*1.5f, 0);
            buryEffectGO.GetComponent<ParticleSystem>().Play();
            Destroy(buryEffectGO, buryEffectGO.GetComponent<ParticleSystem>().main.duration + buryEffectGO.GetComponent<ParticleSystem>().main.startLifetime.constantMax);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * burrowSpeed;
            enemy.transform.position = Vector3.Lerp(maskCenter, riseTarget, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * jumpSpeed;
            enemy.transform.position = Vector3.Lerp(riseTarget, originalPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        health.enabled = true;
        damage.enabled = true;
        boxCollider.enabled = true;

        isBurried?.Invoke(false);

        RefreshSpritesForMasking();

        mask.SetActive(false);
    }

    private void RefreshSpritesForMasking()
    {
        var renderers = enemy.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in renderers)
        {
            sr.enabled = false;
            sr.enabled = true;
        }
        mask.GetComponent<SpriteMask>().frontSortingLayerID = SortingLayer.NameToID("Background"); 
        mask.GetComponent<SpriteMask>().frontSortingLayerID = SortingLayer.NameToID("Default");

    }
}