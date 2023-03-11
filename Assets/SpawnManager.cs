using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public BoxCollider boxCollider;
    public GameObject[] spawnPoints;

    public GameObject enemyPrefab;

    [Header("SpawnManagement")]
    public float timeSinceLastSpawn;
    public float spawnInterval;
    public int spawnCount;
    public int spawnLimit;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = this.GetComponent<BoxCollider>();
        timeSinceLastSpawn = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }

    public void SpawnEnemies(float spawnInterval)
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > spawnInterval)
        {
            InstantiateEnemy(enemyPrefab, 1);
            timeSinceLastSpawn = 0;
        }
    }
    public void InstantiateEnemy(GameObject enemyPrefab, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].transform.position, Quaternion.identity);
            spawnCount++;
        }
    }
    public bool TakeChance(float probability)
    {
        return probability > Random.Range(0, 1);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (spawnCount < spawnLimit)
            {
                SpawnEnemies(spawnInterval);
            }
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SpawnManager : MonoBehaviour
//{
//    public BoxCollider boxCollider;
//    public Vector3[] spawnPoints;

//    public GameObject enemyPrefab;

//    [Header("SpawnRate")]
//    float currentTimer;
//    float spawnRate;
//    // Start is called before the first frame update
//    void Start()
//    {
//        boxCollider = this.GetComponent<BoxCollider>();
//        currentTimer = 0;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireCube(transform.position, boxCollider.size);
//    }

//    public void SpawnEnemies(float spawnRate)
//    {
//        currentTimer += Time.deltaTime;
//        if (currentTimer > spawnRate)
//        {
//            InstantiateEnemy(enemyPrefab, 5);
//            currentTimer = 0;
//        }
//    }
//    public void InstantiateEnemy(GameObject enemyPrefab, int spawnAmount)
//    {
//        for (int i = 0; i < spawnAmount; i++)
//        {
//            int spawnIndex = Random.Range(0, spawnPoints.Length);
//            Instantiate(enemyPrefab, spawnPoints[spawnIndex], Quaternion.identity);
//        }
//    }
//    public bool TakeChance(float probability)
//    {
//        return probability > Random.Range(0, 1);
//    }

//    private void OnTriggerStay(Collider other)
//    {
//        if(other.tag == "Player")
//        {
//            SpawnEnemies(spawnRate);
//        }
//    }
//}
