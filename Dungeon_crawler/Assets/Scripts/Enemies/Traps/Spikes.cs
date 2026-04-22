using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float delayBeforeActivation = 0.5f;
    [SerializeField] private float duration = 1f;

    private Transform spikes;
    private BoxCollider2D hitbox;

    private bool playerInside;
    private bool isActive;

    private Coroutine activateRoutine;
    private Coroutine deactivateRoutine;

    void Start()
    {
        spikes = transform.Find("Spikes");
        hitbox = spikes.Find("Hitbox").GetComponent<BoxCollider2D>();

        spikes.gameObject.SetActive(false);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInside = true;

        if (deactivateRoutine != null)
        {
            StopCoroutine(deactivateRoutine);
            deactivateRoutine = null;
        }

        if (!isActive && activateRoutine == null)
        {
            activateRoutine = StartCoroutine(Activate());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInside = false;

        if (isActive && deactivateRoutine == null)
        {
            deactivateRoutine = StartCoroutine(Deactivate());
        }
    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(delayBeforeActivation);

        isActive = true;
        spikes.gameObject.SetActive(true);
        hitbox.enabled = true;

        activateRoutine = null;

        if (!playerInside && deactivateRoutine == null)
        {
            deactivateRoutine = StartCoroutine(Deactivate());
        }
    }

    private IEnumerator Deactivate()
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (playerInside)
            {
                deactivateRoutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isActive = false;
        spikes.gameObject.SetActive(false);
        hitbox.enabled = false;

        deactivateRoutine = null;
    }
}