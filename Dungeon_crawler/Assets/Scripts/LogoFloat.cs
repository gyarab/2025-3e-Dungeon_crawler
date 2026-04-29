using UnityEngine;

public class LogoFloat : MonoBehaviour
{
    public float speed = 1f;
    public float height = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, Mathf.Sin(Time.time) * height, 0) * Time.deltaTime*speed;
    }
}
