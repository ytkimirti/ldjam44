using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject healthBarPrefab;

    public static GameManager main;

    void Awake()
    {
        main = this;
    }

    void Start()
    {

    }


    void Update()
    {

    }
}
