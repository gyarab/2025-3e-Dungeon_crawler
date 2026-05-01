using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private Coroutine shakeRoutine;
    public ParticleSystem shakeEffect;


    public void Shake(float duration, float intensity)
    {
        //shakes the camera, the intensity is divided by 10 to make it less extreme, and the shake effect is played above the screen
        intensity /= 10;
        Camera cam = Camera.main;

        float yAboveScreen = cam.transform.position.y + cam.orthographicSize + 0.5f;

        Vector3 spawnPos = new Vector3(cam.transform.position.x, yAboveScreen, 0f);
        //spawns the shake effect above the screen
        ParticleSystem effect = Instantiate(shakeEffect, spawnPos, Quaternion.identity);
        effect.transform.parent = cam.transform;

        effect.Play();
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float elapsed = 0f;
        //moves the camera randomly within a circle
        while (elapsed < duration)
        {
            originalPosition = transform.position;
            Vector2 offset = Random.insideUnitCircle * intensity;
            transform.position = originalPosition + (Vector3)offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        shakeRoutine = null;
    }
}