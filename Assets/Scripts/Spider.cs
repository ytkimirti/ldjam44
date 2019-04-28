using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class Spider : Entity
{
    public bool isPlayer;
    public float moveSpeed;
    public float maxSpeed;
    public Color color;
    public Color secondColor;
    [Space]
    public float moveSpeedIncrease;
    [Space]
    public float damage;
    [Space]
    public float damageIncrease;
    public float dashSpeed;
    public float dashDelay;
    float dashTimer;

    public float attackDelay;
    float attackTimer;

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
    public bool isShielding;


    [Header("References")]
    public SpriteRenderer mainSprite;
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

        SetColor(color, secondColor);
    }

    void SpawnParts()
    {

        SpawnLimbs(true);
        SpawnLimbs(false);

        SpawnEyes();
    }

    public void MetalEffect()
    {
        foreach (Limb arm in arms)
        {
            arm.metalParticle.Play();
        }
    }

    public void ShakeEffect()
    {
        foreach (Eye eye in eyes)
        {
            eye.Wink();
        }

        mainSprite.color = GameManager.main.damagedColor;
        mainSprite.DOColor(color, 0.1f).SetDelay(0.2f);
        visualParent.transform.localPosition = Random.insideUnitCircle * 0.3f;
    }

    public override void AddGore(Vector2 attackPos)
    {

        if (isShielding)
            return;

        ShakeEffect();

        Vector2 diff = ((Vector2)attackPos - (Vector2)transform.position).normalized * Random.Range(0f, 0.5f);

        float diffAngle = diff.ToAngle();

        ParticleManager.main.play(diff + (Vector2)visualParent.position, new Vector3(0, 0, diffAngle), 1);

        Instantiate(GameManager.main.gorePrefab, diff + (Vector2)visualParent.position, Quaternion.Euler(0, 0, diffAngle), mainSprite.transform);
    }

    public override void AttackArea(Vector2 pos, float damage)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, searchArea, attackLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D col = colliders[i];

            if (!col || col.gameObject == this.gameObject)
                return;

            Health hp = col.gameObject.GetComponent<Health>();

            if (hp)
            {
                hp.GetDamage(damage);

                if (hp.GetComponent<Entity>())
                {
                    hp.GetComponent<Entity>().AddGore(transform.position);
                }
            }
        }
    }

    public override void GetDamage(float amount)
    {
        if (isShielding)
        {
            MetalEffect();
            return;
        }
        base.GetDamage(amount);
    }

    public void SetColor(Color col, Color sec)
    {
        foreach (Limb arm in arms)
        {
            arm.SetColor(col, sec);
        }

        foreach (Eye eye in eyes)
        {
            eye.SetColor(col);
        }
        foreach (Limb arm in legs)
        {
            arm.SetColor(col, sec);
        }
        mainSprite.color = col;

    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (isPlayer)
            isShielding = Input.GetKey(KeyCode.Mouse1);

        UpdateHealthBar();

        sort.sortingOrder = Mathf.RoundToInt(transform.position.y * -10);

        Vector2 target = Vector2.ClampMagnitude((input.targetInput - (Vector2)transform.position) / 3, lerpAmount);

        visualParent.localPosition = Vector2.Lerp(visualParent.localPosition, target, lerpSpeed * Time.deltaTime);
    }

    public override void OnAttackButtonPressed()
    {
        if (attackTimer <= 0 && Vector2.Distance((Vector2)transform.position, input.targetInput) < meleeRadius)
        {
            attackTimer = attackDelay;
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
                limb.limbType = "arm_knife";
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

        if (dashTimer < dashDelay / 2)
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0 && Input.GetKeyDown(KeyCode.Space) && isPlayer)
        {
            rb.velocity = dashSpeed * input.movementInput;
            dashTimer = dashDelay;
        }
    }
}
