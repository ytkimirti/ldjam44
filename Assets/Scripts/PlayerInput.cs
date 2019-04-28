using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : CharacterInput
{

    Camera cam;

    void Start()
    {
        GetComponent<Spider>().isPlayer = true;
        cam = CameraController.main.cam;
        Init();
    }

    void Update()
    {

        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        targetInput = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnAttackButtonPressed();
        }
    }
}
