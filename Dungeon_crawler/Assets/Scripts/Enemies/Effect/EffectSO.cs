using UnityEngine;

[CreateAssetMenu(fileName = "EffectSO", menuName = "Effects/Status Effect")]
public class EffectSO : ScriptableObject
{
    //information for effects like poison or fire
    //speed modifiers, attack speed modifiers not implemented
    public string effectName;
    public float duration;
    public float healthChangePerTick;
    public float tickInterval;
    public float speedModifier;
    public float attackSpeedModifier;
    public Color CameraColorEffect;
    public ParticleSystem particles;
}
