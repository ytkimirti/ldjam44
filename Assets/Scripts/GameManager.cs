using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Spider player;
    public List<Entity> entityList;
    public Vector2 scaleRand;
    public Vector2 movementSpeedRand;
    public Vector2 maxSpeedRand;
    public Vector2 hpRand;
    public Vector2 attackSpeedRand;
    public Vector2 damageRand;
    public TextMeshProUGUI currentHealth;
    public GameObject[] leafPrefab;
    public int[] leafCount;

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

    public float movementSpeedPerLeg;
    public float maxSpeedPerLeg;
    public float damagePerArm;
    public float sightOverEye;
    public float areaIncreaseOverEye;
    public float defArea;
    public float defSight;
    public Animator endGameAnim;
    public TextMeshProUGUI killText;

    public PixelCamera pixelCam;

    public int killCount;

    public Transform cameraTopTrans;

    [Space]

    public static GameManager main;

    public bool isGameEnded = false;

    void Awake()
    {
        main = this;
    }

    public void EndGame()
    {
        if (isGameEnded)
            return;

        isGameEnded = true;

        endGameAnim.SetTrigger("end");

        killText.text = "Thus, you started a \nhuge chaos and killed \n" + killCount.ToString() + " spiders";

        Destroy(Camera.main.GetComponent<AudioListener>());
    }

    void Start()
    {
        /*
        for (int j = 0; j < leafCount.Length; j++)
        {
            for (int i = 0; i < leafCount[i]; i++)
            {
                Instantiate(leafPrefab[j], spawnRadius * Random.insideUnitCircle, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }
        }
         */


        SpawnSpiders(10);

        defSight = pixelCam.pixelsPerUnit;
        defArea = player.searchArea;

        movementSpeedPerLeg = player.moveSpeed / player.legCount;
        maxSpeedPerLeg = player.maxSpeed / player.legCount;

        damagePerArm = player.damage / player.armCount;
    }

    public void IncreaseHP(float amount)
    {
        player.currentHealth += amount;

        Drop(Mathf.RoundToInt(amount), cameraTopTrans.position, player.transform, 0.5f);
    }

    public void DecreaseHP(float amount)
    {
        player.currentHealth -= amount;

        Drop(Mathf.RoundToInt(amount), player.transform.position, cameraTopTrans, 0.5f);
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(0);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Camera.main.GetComponent<AudioListener>().enabled = !Camera.main.GetComponent<AudioListener>().enabled;
        }

        if (player)
        {
            currentHealth.text = Mathf.RoundToInt(player.currentHealth).ToString();
        }

        if (entityList.Count < minSpiderCount)
        {
            SpawnRandomSpider();
        }
    }
}
