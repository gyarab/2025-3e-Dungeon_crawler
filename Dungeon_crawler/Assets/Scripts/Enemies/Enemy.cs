using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public, private, protected
    //private = zadny tridy, zadny inspektor
    [Header("Basic settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject player;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = player.GetComponent<Transform>();
    }

    private void Update()
    {
        Transform enemyTransform = gameObject.transform;
        Vector3 direction = playerTransform.position - enemyTransform.position;
        direction = Vector3.Normalize(direction);
        gameObject.GetComponent<Rigidbody2D>().AddForce(direction);
    }
}
