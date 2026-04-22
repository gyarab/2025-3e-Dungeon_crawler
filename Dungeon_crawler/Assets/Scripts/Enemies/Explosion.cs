using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private List<ParticleSystem> explosionEffect;
    [SerializeField] private float cameraShakeIntensity;
    [SerializeField] private float explosionDuration = 1f;
    [Header("Damage")]
    [SerializeField] private int damage;
    [SerializeField] private int knockbackForce;
    [SerializeField] private List<string> unhittableTags = new List<string>();

    public void Explode()
    {
        GameObject explosion = Instantiate(new GameObject("Explosion"),transform.position, Quaternion.identity).gameObject;
        foreach(ParticleSystem ps in explosionEffect)
        {
            ParticleSystem effect = Instantiate(ps, explosion.transform.position, Quaternion.identity);
            effect.transform.position = new Vector3(explosion.transform.position.x, explosion.transform.position.y, explosion.transform.position.z + 0.1f);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        CameraShake cameraShakeInstance = Camera.main.GetComponent<CameraShake>();
        if (cameraShakeInstance != null)
        {
            cameraShakeInstance.Shake(0.5f, cameraShakeIntensity);
        }

        CircleCollider2D explosionCollider = explosion.AddComponent<CircleCollider2D>();
        explosionCollider.radius = radius;
        explosionCollider.isTrigger = true;
        Damage dmg = explosionCollider.gameObject.AddComponent<Damage>();
        dmg.SetDamage(damage);
        dmg.SetKnockbackForce(knockbackForce);
        dmg.SetUnhittableTags(unhittableTags);
        Destroy(explosion, explosionDuration);
    }
}
