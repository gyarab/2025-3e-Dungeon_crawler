using UnityEngine;

public class LogoFloat : MonoBehaviour
{
    public float speed = 1f;
    public float height = 0.5f;
    // Update is called once per frame
    void Update()
    {
        //makes the logo go up and down
        transform.position += new Vector3(0, Mathf.Sin(Time.time) * height, 0) * Time.deltaTime*speed;
    }
}
