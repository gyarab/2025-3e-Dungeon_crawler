using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public String targetScene;
    public void Change()
    {
        SceneManager.LoadScene(targetScene);
    }
}
