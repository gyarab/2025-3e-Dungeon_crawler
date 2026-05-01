using UnityEngine;

public class RandomShake : MonoBehaviour
{
    public float shakeChance = 0.01f;
    public float minShakeDuration = 0.05f;
    public float maxShakeDuration = 0.5f;
    public float minShakeIntensity = 0.05f;
    public float maxShakeIntensity = 0.5f;
    void Update()
    {
        //randomly shakes the camera
        if (Random.Range(0f, 1f) < shakeChance) 
        {
            Camera.main.GetComponent<CameraShake>().Shake(Random.Range(minShakeDuration, maxShakeDuration),Random.Range(minShakeIntensity, maxShakeIntensity));
        }
    }
}
