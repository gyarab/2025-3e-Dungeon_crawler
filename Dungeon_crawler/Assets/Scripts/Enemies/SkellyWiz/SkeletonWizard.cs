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

    [SerializeField] private float teleportOutTime = 0.4f;
    [SerializeField] private float teleportInTime = 0.4f;
    [SerializeField] private float shootTime = 0.6f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Loop());
    }

    void Update()
    {
        FacePlayer();
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(idleMin, idleMax));

            animator.SetTrigger("TeleportOut");
            yield return new WaitForSeconds(teleportOutTime);

            TeleportToRandomFloor();

            animator.SetTrigger("TeleportIn");
            yield return new WaitForSeconds(teleportInTime);

            animator.SetTrigger("Shoot");
            SpawnProjectile();
            yield return new WaitForSeconds(shootTime);
        }
    }

    void TeleportToRandomFloor()
    {
        var room = transform.parent.GetComponent<Room>();

        if (room == null)
        {
            Debug.LogError("Room is NULL");
            return;
        }

        if (room.accessableFloors == null)
        {
            Debug.LogError("accessableFloors is NULL");
            return;
        }

        if (room.accessableFloors.Count == 0)
        {
            Debug.LogError("accessableFloors is EMPTY");
            return;
        }

        var accessableFloors = room.accessableFloors;
        var floorList = new List<Vector2Int>(accessableFloors);

        Vector2Int randomFloor = floorList[Random.Range(0, floorList.Count)];

        Debug.Log("Teleporting to: " + randomFloor);

        transform.position = new Vector3(randomFloor.x, randomFloor.y, 0);
    }

    void SpawnProjectile()
    {
        if (!projectilePrefab || !player || !projectileSpawn)
            return;

        GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity);
        Vector2 dir = (player.position - projectileSpawn.position).normalized;
        proj.transform.up = dir;
    }

    void FacePlayer()
    {
        if (!player) return;

        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}