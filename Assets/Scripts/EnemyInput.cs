using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharacterInput
{
    public Transform target;

    void Start()
    {

    }

    void Update()
    {
        if (target)
        {
            movementInput = (target.position - transform.position);
        }
    }
}
