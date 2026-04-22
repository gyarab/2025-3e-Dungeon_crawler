using System.Collections;
using UnityEngine;

public class EffectInstance : MonoBehaviour
{
    public EffectSO effectData;

    private Health healthComponent;
    private float elapsedTime = 0;

    private string id;
    private ParticleSystem ps;

    void Start()
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

    IEnumerator Effect()
    {
        if(effectData.particles != null)
        {
            ps = Instantiate(effectData.particles, new Vector3(transform.position.x,transform.position.y,transform.position.z-2), Quaternion.identity);
            
            ps.transform.SetParent(transform);
            ps.Play();
        }
        if(gameObject.tag== "Player")
        {
            id = Camera.main.GetComponent<CameraTint>().GetFreeID();
            Camera.main.GetComponent<CameraTint>().AddTint(id, effectData.CameraColorEffect);
        }
        while (elapsedTime < effectData.duration)
        {
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
