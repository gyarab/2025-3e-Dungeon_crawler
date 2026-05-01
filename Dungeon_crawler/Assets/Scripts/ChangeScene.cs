using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public String targetScene;
    public void Change()
    {
        //changes the scene based on string name
        SceneManager.LoadScene(targetScene);
        Time.timeScale = 1f;
    }
}
