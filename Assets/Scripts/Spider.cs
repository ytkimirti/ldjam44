using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public float moveSpeed;
    public float maxSpeed;

    CharacterInput input;
    Rigidbody2D rb;

    void Start()
    {
        input = GetComponent<CharacterInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.AddForce(moveSpeed * input.movementInput);

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }
}
