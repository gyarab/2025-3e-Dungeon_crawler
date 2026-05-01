using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float lookAheadDistance = 2f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastTargetPosition;

    void Start()
    {
        lastTargetPosition = target.position;
    }

    void LateUpdate()
    {
        //targets the player
        if (target == null) return;

        Vector3 delta = target.position - lastTargetPosition;
        Vector3 lookAhead = delta.normalized * lookAheadDistance;
        //looks ahead of the player based on the direction they are moving in, and then smoothly moves towards that position
        Vector3 targetPos = new Vector3( target.position.x + lookAhead.x, target.position.y + lookAhead.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        lastTargetPosition = target.position;
    }
}
