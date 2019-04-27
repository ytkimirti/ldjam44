using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public Transform eye_white;
    public Transform eye_black;

    public float moveAmount;
    public float blackMoveAmount;

    CharacterInput input;

    void Start()
    {
        input = GetComponentInParent<CharacterInput>();
    }

    void Update()
    {
        Vector2 localMovement = input.targetInput - (Vector2)transform.position;

        localMovement.Normalize();

        eye_white.transform.localPosition = Vector2.ClampMagnitude(localMovement / 5, moveAmount);

        eye_black.transform.localPosition = Vector2.ClampMagnitude(localMovement / 5, blackMoveAmount);
    }
}
