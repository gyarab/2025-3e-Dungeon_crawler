using System.Collections;
using UnityEngine;

public class EffectInstance : MonoBehaviour
{
    //applies the effect of an EffectSO to the gameobject it is attached to, then destroys itself after the duration of the effect is over
    public EffectSO effectData;

    private Health healthComponent;
    private float elapsedTime = 0;

    private string id;
    private ParticleSystem ps;

    void Start()
    {
        try
        {
            healthComponent = GetComponentInChildren<Health>() ?? GetComponent<Health>();
            StartCoroutine(Effect());

            //remove this component if gameobject already has the same effect
            EffectInstance[] existingEffects = GetComponents<EffectInstance>();
            foreach (EffectInstance effect in existingEffects)
            {
                if (effect != this && effect.effectData == effectData)
                {
                    Destroy(this);
                    return;
                }
            }
        }
        catch (System.Exception e)
        {
            Destroy(this);
        }

        IEnumerator Effect()
        {
            if (effectData.particles != null)
            {
                //particle effects
                ps = Instantiate(effectData.particles, new Vector3(transform.position.x, transform.position.y, transform.position.z - 2), Quaternion.identity);

                ps.transform.SetParent(transform);
                ps.Play();
            }
            if (gameObject.tag == "Player")
            {
                //camera tint effect for player
                id = Camera.main.GetComponent<CameraTint>().GetFreeID();
                Camera.main.GetComponent<CameraTint>().AddTint(id, effectData.CameraColorEffect);
            }
            while (elapsedTime < effectData.duration)
            {
                //damage over time or heal over time
                if (healthComponent != null)
                {
                    healthComponent.ChangeHealth((int)effectData.healthChangePerTick);
                }
                elapsedTime += effectData.tickInterval;
                yield return effectData.tickInterval;
            }
            Camera.main.GetComponent<CameraTint>().RemoveTint(id);
            Destroy(this);
            Destroy(ps);
        }
    }
}