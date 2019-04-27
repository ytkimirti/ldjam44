using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : CharacterInput
{
    void Start()
    {

    }

    void Update()
    {
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }
}
