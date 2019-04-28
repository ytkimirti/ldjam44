using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Color damagedColor;
    public GameObject healthBarPrefab;
    public GameObject gorePrefab;

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
