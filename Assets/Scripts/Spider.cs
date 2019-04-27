using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class Spider : Entity
{
    public float moveSpeed;
    public float maxSpeed;

    [Header("Limbs")]
    public int armCount;
    public int legCount;
    public int eyeCount;
    [Space]
    public Transform[] legSpots;
    public Transform[] armSpots;
    public Transform[] eyeSpots;
    [Space]
    Eye[] eyes;
    Limb[] arms;
    Limb[] legs;


    [Header("References")]
    public GameObject limbPrefab;
    public GameObject eyePrefab;
    public Transform visualParent;

    CharacterInput input;
    SortingGroup sort;
    Rigidbody2D rb;

    void Start()
    {
        sort = GetComponentInChildren<SortingGroup>();
        input = GetComponent<CharacterInput>();
        rb = GetComponent<Rigidbody2D>();

        SpawnParts();
    }

    void SpawnParts()
    {
        SpawnLimbs(true);
        SpawnLimbs(false);

        SpawnEyes();
    }

    void Update()
    {
        sort.sortingOrder = Mathf.RoundToInt(transform.position.y / -10);
    }

    void SpawnLimbs(bool isLeg)
    {
        int count = isLeg ? legCount : armCount;

        if (isLeg)
            legs = new Limb[count];
        else
            arms = new Limb[count];

        for (int i = 0; i < count; i++)
        {
            Transform spot = isLeg ? legSpots[i] : armSpots[i];

            GameObject limbGO = Instantiate(limbPrefab, spot.position, Quaternion.identity, visualParent);

            Limb limb = limbGO.GetComponent<Limb>();

            limb.isWalking = isLeg;
            limb.isArm = !isLeg;

            if (isLeg)
                legs[i] = limb;
            else
                arms[i] = limb;
        }
    }

    void SpawnEyes()
    {
        eyes = new Eye[eyeCount];

        for (int i = 0; i < eyeCount; i++)
        {
            GameObject eyeGO = Instantiate(eyePrefab, eyeSpots[i].position, Quaternion.identity, visualParent);

            Eye eye = eyeGO.GetComponent<Eye>();

            eyes[i] = eye;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(moveSpeed * input.movementInput);

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }
}
