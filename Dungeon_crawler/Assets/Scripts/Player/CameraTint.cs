using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTint : MonoBehaviour
{
    [SerializeField] private float sizePadding = 0.5f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private string sortingLayerName = "Overlay";
    [SerializeField] private int baseSortingOrder = 1000;

    private Camera cam;
    private Sprite generatedSprite;

    private Dictionary<string, GameObject> activeTints = new();
    private Dictionary<string, SpriteRenderer> tintRenderers = new();
    private Dictionary<string, Coroutine> fadeCoroutines = new();

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            enabled = false;
            return;
        }

        generatedSprite = CreateWhiteSprite();
    }

    public string GetFreeID()
    {
        int id = 0;
        while (true)
        {
            string key = id.ToString();
            if (!activeTints.ContainsKey(key))
                return key;
            id++;
        }
    }

    public void AddTint(string id, Color color)
    {
        if (activeTints.ContainsKey(id))
            return;

        GameObject tintGO = new GameObject($"Tint_{id}");
        tintGO.transform.SetParent(transform);
        tintGO.transform.localPosition = new Vector3(0f, 0f, 1f);
        tintGO.transform.localRotation = Quaternion.identity;

        SpriteRenderer sr = tintGO.AddComponent<SpriteRenderer>();
        sr.sprite = generatedSprite;
        sr.color = new Color(color.r, color.g, color.b, 0f);
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = baseSortingOrder + activeTints.Count;

        ResizeToCamera(sr);

        activeTints[id] = tintGO;
        tintRenderers[id] = sr;

        fadeCoroutines[id] = StartCoroutine(Fade(sr, 0f, color.a));
    }

    public void RemoveTint(string id)
    {
        if (!activeTints.TryGetValue(id, out GameObject tintGO))
            return;

        SpriteRenderer sr = tintRenderers[id];

        if (fadeCoroutines.TryGetValue(id, out Coroutine c))
            StopCoroutine(c);

        fadeCoroutines[id] = StartCoroutine(FadeAndDestroy(id, sr, tintGO));
    }

    private IEnumerator Fade(SpriteRenderer sr, float from, float to)
    {
        float t = 0f;
        Color c = sr.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            sr.color = c;
            yield return null;
        }

        c.a = to;
        sr.color = c;
    }

    private IEnumerator FadeAndDestroy(string id, SpriteRenderer sr, GameObject go)
    {
        float startAlpha = sr.color.a;
        float t = 0f;
        Color c = sr.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            sr.color = c;
            yield return null;
        }

        Destroy(go);
        activeTints.Remove(id);
        tintRenderers.Remove(id);
        fadeCoroutines.Remove(id);
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    private void ResizeToCamera(SpriteRenderer sr)
    {
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector2 spriteSize = sr.sprite.bounds.size;

        sr.transform.localScale = new Vector3(
            camWidth / spriteSize.x + sizePadding,
            camHeight / spriteSize.y + sizePadding,
            1f
        );
    }
}