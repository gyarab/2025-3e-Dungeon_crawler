using UnityEngine;

public class Pause : MonoBehaviour
{
    private void Update()
    {
        //if escape is pressed, toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    public void Show()
    {
        Time.timeScale = 0f; // Pause the game
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        Time.timeScale = 1f; // Resume the game
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
