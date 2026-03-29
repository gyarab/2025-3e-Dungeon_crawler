using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public float speed = 2f;
    public Transform target;
    public bool isBurried = false;

    private void Start()
    {
        transform.GetComponent<Burrow>().isBurried.AddListener((burried) => { isBurried = burried; });
    }

    void Update()
    {
        if (target == null||isBurried)
            return;
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
