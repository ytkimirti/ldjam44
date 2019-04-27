using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

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

    void Update()
    {

    }
}