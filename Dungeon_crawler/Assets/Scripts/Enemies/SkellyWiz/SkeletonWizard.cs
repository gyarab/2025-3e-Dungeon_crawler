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
    [SerializeField] private float delayBeforeShoot = 0.4f;

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
        // Behaviour loop: Idle - Teleport Out - Teleport In - Shoot - Idle
        while (true)
        {
            //Idle for a random duration
            yield return new WaitForSeconds(Random.Range(idleMin, idleMax));

            // Play teleport out animation and wait for it to finish
            animator.SetTrigger("TeleportOut");
            yield return new WaitForSeconds(teleportOutTime);
            
            // Teleport method
            TeleportToRandomFloor();

            // Play teleport in animation and wait for it to finish
            animator.SetTrigger("TeleportIn");
            yield return new WaitForSeconds(teleportInTime);


            // Projectile attack animation and projectile spawning
            animator.SetTrigger("Shoot");
            yield return new WaitForSeconds(delayBeforeShoot);
            SpawnProjectile();

            yield return new WaitForSeconds(shootTime);
        }
    }

    void TeleportToRandomFloor()
    {
        var room = transform.parent.GetComponent<Room>();

    

        // Gets random floor tile from the room and teleports to that position
        var accessableFloors = room.accessableFloors;
        var floorList = new List<Vector2Int>(accessableFloors);

        Vector2Int randomFloor = floorList[Random.Range(0, floorList.Count)];

        transform.position = new Vector3(randomFloor.x, randomFloor.y, 0);
    }

    void SpawnProjectile()
    {
       

        // Instantiate projectile from prefab at projectile spawn game object position
        GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity);

        // Set direction of projectile towards player
        Vector2 dir = (player.position - projectileSpawn.position).normalized;

        proj.transform.up = dir;

        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDirection(dir);
        }
    }

    void FacePlayer()
    {
        
        // Flip sprite based on player position
        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}