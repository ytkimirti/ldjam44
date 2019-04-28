using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Entity> entityList;
    public Color damagedColor;
    public GameObject healthBarPrefab;
    public GameObject gorePrefab;
    public Transform canvasParent;

    public Color[] spiderColors;

    [Header("Stats")]

    public float movementSpeedIncreaseOverLeg;

    public float sightOverEye;

    public float speedOverScale;

    public float damageOverArm;
    [Space]
    public float legCost;
    public float eyeCost;
    public float armCost;

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
