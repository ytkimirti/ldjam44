using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class Spider : Entity
{
    public float moveSpeed;
    public float maxSpeed;
    public float damage;

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
    public float lerpSpeed;
    public float lerpAmount;

    CharacterInput input;
    SortingGroup sort;
    Rigidbody2D rb;

    void Start()
    {
        sort = GetComponentInChildren<SortingGroup>();
        input = GetComponent<CharacterInput>();
        rb = GetComponent<Rigidbody2D>();

        SpawnParts();
        SpawnHealthBar();
    }

    void SpawnParts()
    {
        SpawnLimbs(true);
        SpawnLimbs(false);

        SpawnEyes();
    }

    void Update()
    {
        UpdateHealthBar();

        sort.sortingOrder = Mathf.RoundToInt(transform.position.y * -10);

        Vector2 target = Vector2.ClampMagnitude((input.targetInput - (Vector2)transform.position) / 3, lerpAmount);

        visualParent.localPosition = Vector2.Lerp(visualParent.localPosition, target, lerpSpeed * Time.deltaTime);
    }

    public override void OnAttackButtonPressed()
    {
        if (Vector2.Distance((Vector2)transform.position, (Vector2)input.targetInput) < meleeRadius)
        {
            AttackLimbs(input.targetInput);
            AttackArea(input.targetInput, damage);
        }
    }

    public void AttackLimbs(Vector2 pos)
    {
        visualParent.localPosition = (input.targetInput - (Vector2)transform.position).normalized * lerpAmount * -1.2f;

        foreach (Limb limb in arms)
        {
            limb.AttackLimb(pos);
        }
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



            if (isLeg)
            {
                limb.limbType = "leg";
                legs[i] = limb;
            }
            else
            {
                limb.limbType = "arm_gun";
                arms[i] = limb;
            }
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
