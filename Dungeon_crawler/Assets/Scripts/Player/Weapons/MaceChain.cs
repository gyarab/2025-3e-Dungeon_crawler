using UnityEngine;

public class MaceChain : MonoBehaviour
{
    private LineRenderer line;
    private float chainTextureUnits = 16f;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        line.SetPosition(0, transform.parent.position);
        line.SetPosition(1, transform.position);

        float distance = Vector3.Distance(transform.parent.position, transform.position);

        line.material.mainTextureScale = new Vector2(distance / chainTextureUnits, 1);
    }
}
