using System.Collections;
using UnityEngine;

public class Stalagtite : MonoBehaviour
{
    [SerializeField] private float finalSpeed = 25f;
    [SerializeField] private float delayBeforeFall = 0.5f;
    [SerializeField] private float heightToDamage = 1.3f;
    public bool playerOnly = true;

    private BoxCollider2D hitbox;
    private bool isFalling = false;

    private Transform stalagtite;
    private ParticleSystem fallEffect;

    void Start()
    {
        hitbox = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
        stalagtite = transform.Find("Stalagtite");
        fallEffect = GetComponentInChildren<ParticleSystem>();
        fallEffect.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (isFalling) return;
        if (!collision.gameObject.CompareTag("Player")&&playerOnly)
        {
            return;

        }
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        isFalling = true;
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;
            stalagtite.position = new Vector3(stalagtite.position.x, cameraTopY + 1f, stalagtite.position.z);
        }

        float distance = stalagtite.localPosition.y;

        float acceleration = (finalSpeed * finalSpeed) / (2f * distance);

        float speed = 0f;

        yield return new WaitForSeconds(delayBeforeFall);
        stalagtite.gameObject.SetActive(true);

        while (stalagtite.localPosition.y > 0)
        {
            speed += acceleration * Time.deltaTime;
            stalagtite.Translate(Vector3.down * speed * Time.deltaTime);
            if(stalagtite.localPosition.y < heightToDamage)
            {
                hitbox.enabled = true;
            }
            yield return null;
        }
        fallEffect.gameObject.SetActive(true);
        fallEffect.transform.parent = null;
        fallEffect.Play();
        Destroy(fallEffect.gameObject, fallEffect.main.duration + fallEffect.main.startLifetime.constantMax);

        Destroy(gameObject);
    }
}
