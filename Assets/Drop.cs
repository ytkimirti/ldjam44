using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class Drop : MonoBehaviour
{
    public float delay;
    float delayTimer;
    public Transform target;
    float currSpeed;
    float currLerp;
    public float momentumOverSecond;
    Vector2 startPos;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        OnSpawned();
    }

    void OnSpawned()
    {
        sprite.enabled = false;
        currLerp = 0;
        currSpeed = 0;
        startPos = transform.position;
        delayTimer = delay;
    }

    void Update()
    {
        delayTimer -= Time.deltaTime;

        if (delayTimer >= 0)
            return;

        if (!target)
        {
            Die();
            return;
        }
        sprite.enabled = true;

        currSpeed += momentumOverSecond * Time.deltaTime;
        currLerp += currSpeed * Time.deltaTime;

        transform.position = Vector2.Lerp(startPos, target.position, currLerp);

        if (currLerp >= 1)
        {
            Die();
        }
    }

    void Die()
    {
        EZ_PoolManager.Despawn(transform);
    }
}
