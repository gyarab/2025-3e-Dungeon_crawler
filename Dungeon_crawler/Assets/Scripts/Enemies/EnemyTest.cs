using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public float speed = 2f;
    public float aggroDistance=20f;
    public Transform target;
    public bool isBurried = false;

    private void Start()
    {
        transform.GetComponent<Burrow>().isBurried.AddListener((burried) => { isBurried = burried; });
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target == null||isBurried||gameObject.GetComponent<Health>().GetHealth()<=0||Vector3.Distance(transform.position, target.position)>aggroDistance)
            return;
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
