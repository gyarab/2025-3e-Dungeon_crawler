using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    //basic enemy that moves towards the target (player) when they are within a certain distance, but only if they are not burried and have health
    public float speed = 2f;
    public float aggroDistance=20f;
    public Transform target;
    public bool isBurried = false;

    private void Start()
    {
        //gets components and sets up listeners
        if(transform.GetComponent<Burrow>() != null)
        {
            transform.GetComponent<Burrow>().isBurried.AddListener((burried) => { isBurried = burried; });
        }
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        //move towards target (player)
        if (target == null||isBurried||Vector3.Distance(transform.position, target.position)>aggroDistance)
            return;
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
