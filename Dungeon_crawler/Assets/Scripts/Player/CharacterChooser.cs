using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterChooser : MonoBehaviour
{
    [SerializeField] private GameObject character;
    private GameObject parent;

    private void Start()
    {
        try
        {
            parent = GameObject.Find("Character Chooser");
        }
        catch
        {
            Debug.LogError("No \"Character Chooser\" game object!");
        }
    }

    public void Select()
    {
        GameManager.Instance.character = character;
        Instantiate(character, transform.position, Quaternion.identity);
        parent.SetActive(false);
    }
}
