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
        //gets references
        spikes = transform.Find("Spikes");
        hitbox = spikes.Find("Hitbox").GetComponent<BoxCollider2D>();

        spikes.gameObject.SetActive(false);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        //only triggers when the player enters, not when they exit, and if the player is already inside, it won't trigger again
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

        //only triggers when the player exits, not when they enter, and if the player is already outside, it won't trigger again
        playerInside = false;

        if (isActive && deactivateRoutine == null)
        {
            deactivateRoutine = StartCoroutine(Deactivate());
        }
    }

    private IEnumerator Activate()
    {
        //waits for the delay before activation, then activates the spikes and hitbox, and if the player is not inside, it starts the deactivate routine
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
        //waits for the duration, then deactivates the spikes and hitbox, and if the player is inside, it stops the deactivate routine
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