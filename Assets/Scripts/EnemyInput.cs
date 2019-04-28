using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharacterInput
{
    public Transform target;
    public float radius;
    public bool alarmed;

    [Header("Random Movement")]
    public float randomMovementDistance;
    public float randomWaitTime;
    [Space]
    public float randomAttackTime;
    float attackTimer;
    Vector2 currTargetPos;
    float waitTimer;
    Spider spider;

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

    void Update()
    {

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
                        if (Random.Range(0, 3) == 1 || (spider && spider.isShielding))
                        {
                            if (spider && spider.isShielding)
                                spider.isShielding = false;
                            else
                                OnAttackButtonPressed();
                        }
                        else
                        {
                            if (spider)
                                spider.isShielding = true;
                        }
                        attackTimer = Random.Range(randomAttackTime / 2, randomAttackTime);
                    }
                }
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
