using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class GameManager : MonoBehaviour
{
    public List<Entity> entityList;
    public Vector2 scaleRand;
    public Vector2 movementSpeedRand;
    public Vector2 maxSpeedRand;
    public Vector2 hpRand;
    public Vector2 attackSpeedRand;
    public Vector2 damageRand;

    [Space]
    public GameObject spiderPrefab;
    public Color damagedColor;
    public GameObject healthBarPrefab;
    public GameObject gorePrefab;
    public Transform canvasParent;
    public Transform dropPrefab;

    public Color[] spiderColors;

    public float minSpiderCount;
    public float spawnRadius;
    public float cameraRadius;

    [Header("Stats")]

    public float movementSpeedIncreaseOverLeg;

    public float sightOverEye;

    public float speedOverScale;

    public float damageOverArm;
    [Space]
    public float legCost;
    public float eyeCost;
    public float armCost;

    [Space]

    public static GameManager main;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        SpawnSpiders(10);
    }

    float GetRandom(Vector2 vec)
    {
        return Random.Range(vec.x, vec.y);
    }

    public void SpawnSpiders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomSpider();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector2.zero, spawnRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Camera.main.transform.position, cameraRadius);
    }

    public void SpawnRandomSpider()
    {
        Vector2 pos = Random.insideUnitCircle * spawnRadius;

        while (Vector2.Distance(pos, (Vector2)CameraController.main.transform.position) < cameraRadius)
        {
            pos = Random.insideUnitCircle * spawnRadius;
        }

        SpawnRandomSpiderInPos(pos);
    }

    public void SpawnRandomSpiderInPos(Vector2 pos)
    {
        SpawnSpider(pos, GetRandom(scaleRand), GetRandom(movementSpeedRand), GetRandom(maxSpeedRand), GetRandom(hpRand), GetRandom(attackSpeedRand), GetRandom(damageRand));
    }

    public void SpawnSpider(Vector2 pos, float scale, float moveSpeed, float maxSpeed, float health, float attackSpeed, float damage)
    {
        GameObject spiderGO = Instantiate(spiderPrefab, pos, Quaternion.identity);

        Spider spider = spiderGO.GetComponent<Spider>();

        spider.damage = damage;
        spider.spiderScale = scale;
        spider.moveSpeed = moveSpeed;
        spider.maxSpeed = maxSpeed;
        spider.currentHealth = health;
        spider.attackDelay = attackSpeed;
    }

    public void Drop(int count, Vector2 from, Transform to, float radius)
    {
        for (int i = 0; i < count; i++)
        {
            Transform drop = EZ_PoolManager.Spawn(dropPrefab, from + radius * Random.insideUnitCircle, Quaternion.identity);

            drop.parent = transform;

            Drop dropScript = drop.GetComponent<Drop>();

            dropScript.target = to;
            dropScript.delay = i * 0.01f;
        }
    }

    void Update()
    {
        if (entityList.Count < minSpiderCount)
        {
            SpawnRandomSpider();
        }
    }
}
