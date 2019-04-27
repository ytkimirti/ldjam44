using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharacterInput
{
    public Transform target;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (target)
        {
            movementInput = (target.position - transform.position).normalized;
            targetInput = target.position;
        }
    }
}
