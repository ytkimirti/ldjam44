using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharacterInput
{
    public Transform target;
    Spider targetScript;
    public float radius;
    public bool alarmed;

    [Header("Random Movement")]
    public float randomMovementDistance;
    public float randomWaitTime;
    [Space]
    public float randomAttackTime;
    public float randomShieldTime;
    float attackTimer;
    Vector2 currTargetPos;
    float waitTimer;
    Spider spider;
    float searchTimer;

    void Start()
    {
        Init();

        spider = GetComponent<Spider>();

        attackTimer = randomAttackTime;
        currTargetPos = randomMovementDistance * Random.insideUnitCircle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    Transform FindClosest()
    {
        float minDist = Mathf.Infinity;
        int minID = -1;

        for (int i = 0; i < GameManager.main.entityList.Count; i++)
        {
            Transform trans = GameManager.main.entityList[i].transform;

            if (trans == transform)
                continue;

            float dist = (trans.position - transform.position).magnitude;

            if (dist < minDist)
            {
                minDist = dist;
                minID = i;
            }
        }

        Transform closest = GameManager.main.entityList[minID].transform;

        targetScript = closest.GetComponent<Spider>();

        return closest;
    }

    void Update()
    {
        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0)
        {
            searchTimer = Random.Range(0.4f, 2);
            target = FindClosest();
        }

        float dist = 0;

        if (target)
            dist = Vector2.Distance(transform.position, target.position);

        alarmed = dist < radius;

        if (target && alarmed)
        {
            movementInput = (target.position - transform.position).normalized;
            targetInput = target.position;

            if (spider)
            {
                if (dist < spider.meleeRadius)
                {
                    movementInput = Random.insideUnitCircle;
                    attackTimer -= Time.deltaTime;

                    if (attackTimer <= 0)
                    {
                        if (Random.Range(0, 10) > 3)
                        {
                            if (spider && spider.isShielding)
                            {
                                spider.isShielding = false;
                            }
                            else
                            {
                                OnAttackButtonPressed();

                            }
                            attackTimer = Random.Range(randomAttackTime / 2, randomAttackTime);
                        }
                        else
                        {
                            if (spider)
                            {
                                spider.isShielding = true;
                                attackTimer = Random.Range(randomShieldTime / 2, randomShieldTime);
                            }
                        }

                    }
                }
            }

            if (targetScript && targetScript.currentHealth > spider.currentHealth)
            {
                movementInput = -movementInput;
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                waitTimer = Random.Range(randomWaitTime / 2, randomWaitTime);
                currTargetPos = randomMovementDistance * Random.insideUnitCircle;
            }

            movementInput = (currTargetPos - (Vector2)transform.position).normalized / 2;
            targetInput = movementInput;
        }
    }
}
