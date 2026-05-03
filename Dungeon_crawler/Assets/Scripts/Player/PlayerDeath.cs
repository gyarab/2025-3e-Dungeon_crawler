using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDeath : MonoBehaviour
{
    public Health playerHealth;
    public GameObject deathScreen;
    public bool isDead = false;

    private void Update()
    {
        if(playerHealth.isDead&&!isDead)
        {
            isDead = true;
            deathScreen.SetActive(true);
            while (Time.timeScale > 0.2)
            {
                Time.timeScale -= Time.deltaTime;
            }
            Camera.main.GetComponent<CameraShake>().Shake(0.2f, 1f);
        }
    }

}
