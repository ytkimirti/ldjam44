using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Vector3 offset;
    public Transform target;
    public float lerpSpeed;
    public float movementAmount;

    [HideInInspector]
    public Camera cam;
    public static CameraController main;

    void Awake()
    {
        cam = Camera.main;
        main = this;
    }

    void Start()
    {

    }

    void LateUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, (Vector2)target.position + (Vector2)offset, lerpSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, transform.position.y, offset.z);
    }
}