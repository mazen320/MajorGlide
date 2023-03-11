using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform player;
    public GameObject prefab;

    public float maxDistance;
    public float minDistance;
    public float spawnInterval;
    public int maxEnemies;

    private float timeOfLastSpawn;

    void Update()
    {
        if (EnemyManager.instances.Count >= maxEnemies) return;
        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceFromPlayer > maxDistance) return;
        if (distanceFromPlayer < minDistance) return;

        if (Time.time - timeOfLastSpawn > spawnInterval)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            timeOfLastSpawn = Time.time;
        }
    }
}