using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonWizard : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawn;

    [SerializeField] private float idleMin = 1.5f;
    [SerializeField] private float idleMax = 3.5f;

    [SerializeField] private float teleportAnimLength = 1f;
    [SerializeField] private float shootAnimLength = 1.2f;
    [SerializeField] private float projectileSpawnDelay = 0.4f;

    void Start()
    {
        StartCoroutine(Loop());
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(idleMin, idleMax));

            animator.SetTrigger("Teleport");
            yield return new WaitForSeconds(teleportAnimLength);

            TeleportToRandomFloor();

            animator.SetTrigger("Shoot");
            StartCoroutine(ShootProjectile());

            yield return new WaitForSeconds(shootAnimLength);
        }
    }

    void TeleportToRandomFloor()
    {
        var accessableFloors = transform.parent.GetComponent<Room>().accessableFloors;
        var floorList = new List<Vector2Int>(accessableFloors);

        if (floorList.Count == 0)
            return;

        Vector2Int randomFloor = floorList[Random.Range(0, floorList.Count)];
        transform.position = new Vector3(randomFloor.x, randomFloor.y, 0);
    }

    IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(projectileSpawnDelay);

        if (projectilePrefab && player && projectileSpawn)
        {
            GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity);
            Vector2 dir = (player.position - projectileSpawn.position).normalized;
            proj.transform.up = dir;
        }
    }
}