using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Vector2 targetInput;
    public Vector2 movementInput;

    Entity entity;

    public void Init()
    {
        print("searchin");
        entity = GetComponent<Entity>();
    }

    void Update()
    {

    }

    public virtual void OnAttackButtonPressed()
    {
        if (entity)
        {
            entity.OnAttackButtonPressed();
        }
    }
}
