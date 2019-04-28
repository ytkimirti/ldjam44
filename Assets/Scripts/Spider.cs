using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.PostProcessing;
using UnityEngine.Rendering;
using EZCameraShake;

public class Spider : Entity
{
    public bool freeze = false;
    public float spiderScale = 1;
    float injureTimer;
    public float injure;
    public bool isPlayer;
    public float moveSpeed;
    public float maxSpeed;
    public Color color;
    public Color secondColor;
    [Space]
    public float damage;
    public float dashSpeed;
    public float dashDelay;
    float dashTimer;

    public float attackDelay;
    float attackTimer;
    public float darken;

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
    public PostProcessingProfile profile;

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
        AddToList();

        color = GameManager.main.spiderColors[Random.Range(0, GameManager.main.spiderColors.Length)];

        secondColor = new Color(color.r * darken, color.g * darken, color.b * darken, 1);

        SetColor(color, secondColor);

        UpdateScale(spiderScale);

        if (isPlayer)
        {
            Invoke("IntroThing", 7.5f);
            Invoke("NoFreeze", 11);
        }
    }

    public void NoFreeze()
    {
        freeze = false;
    }

    public void IntroThing()
    {
        AttackArea(Vector2.zero, 40, true);

    }

    public void UpdateParts()
    {
        legCount = Mathf.Clamp(legCount, 0, legSpots.Length);
        armCount = Mathf.Clamp(armCount, 0, armSpots.Length);
        eyeCount = Mathf.Clamp(eyeCount, 0, eyeSpots.Length);

        DestroyLimbs();

        SpawnParts();

        SetColor(color, secondColor);

        if (isPlayer)
            GameManager.main.pixelCam.pixelsPerUnit = Mathf.RoundToInt(GameManager.main.sightOverEye * eyeCount + GameManager.main.defSight);

        searchArea = GameManager.main.defArea + GameManager.main.areaIncreaseOverEye * eyeCount;

        if (isPlayer && eyeCount == 1)
        {
            profile.motionBlur.enabled = true;
            profile.chromaticAberration.enabled = true;
            profile.bloom.enabled = false;
            profile.grain.enabled = false;
            profile.vignette.enabled = true;
        }
        else if (isPlayer && eyeCount == 0)
        {
            profile.motionBlur.enabled = true;
            profile.chromaticAberration.enabled = true;
            profile.bloom.enabled = true;
            profile.grain.enabled = true;
            profile.vignette.enabled = true;
        }
        else
        {
            profile.motionBlur.enabled = false;
            profile.chromaticAberration.enabled = false;
            profile.bloom.enabled = false;
            profile.grain.enabled = false;
            profile.vignette.enabled = false;
        }

        moveSpeed = GameManager.main.movementSpeedPerLeg * legCount;
        maxSpeed = GameManager.main.maxSpeedPerLeg * legCount;

        damage = GameManager.main.damagePerArm * armCount;
    }

    public override void UpdateHealthBar()
    {
        base.UpdateHealthBar();

        healthBar.currentInjure = Mathf.RoundToInt(injure);
    }

    public void DestroyLimbs()
    {
        foreach (Limb limb in legs)
        {
            Destroy(limb.gameObject);
        }

        foreach (Limb limb in arms)
        {
            Destroy(limb.gameObject);
        }

        foreach (Eye eye in eyes)
        {
            Destroy(eye.gameObject);
        }

        eyes = new Eye[0];
        legs = new Limb[0];
        arms = new Limb[0];
    }

    public void UpdateScale(float scale)
    {
        maxSpeed /= scale;
        moveSpeed /= scale;

        currentHealth *= scale;
        damage *= scale;
        attackDelay /= scale;

        transform.localScale = Vector3.one * scale;
    }

    public void ClearInjures()
    {
        injure /= 2;

        int times = mainSprite.transform.childCount / 2;

        foreach (Transform child in mainSprite.transform)
        {
            times--;
            if (times <= 0)
                break;
            Destroy(child.gameObject);
        }
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

    public override void Die()
    {
        AudioManager.main.Play("die");
        ParticleManager.main.play(transform.position, Vector2.zero, 2);

        base.Die();
    }

    public override void AttackArea(Vector2 pos, float damage, bool isUnique)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, isUnique ? 3 : searchArea, attackLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D col = colliders[i];

            if (!col)
                return;

            if (!isUnique && col.gameObject == this.gameObject)
                return;

            Health hp = col.gameObject.GetComponent<Health>();

            if (hp)
            {
                bool isHit = hp.GetDamage(damage);

                if (hp.GetComponent<Entity>())
                {
                    hp.GetComponent<Entity>().AddGore(transform.position);
                }

                if (isUnique && isPlayer)
                    AudioManager.main.Play("gore");

                if (isHit && !isUnique)
                {
                    GameManager.main.Drop(Mathf.RoundToInt(damage), hp.transform.position, transform, hp.transform.localScale.x / 2);
                    currentHealth += damage;

                    CheckHealth();

                    if (isPlayer)
                        AudioManager.main.Play("gore");
                }
            }
        }
    }

    public override bool GetDamage(float amount)
    {


        if (isShielding)
        {
            if (isPlayer)
                AudioManager.main.Play("metal");

            MetalEffect();
            return false;
        }

        if (isPlayer)
            AudioManager.main.Play("gore");

        injure += Mathf.RoundToInt(amount / 20);

        base.GetDamage(amount);

        return true;
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

        if (injure != 0)
        {
            injureTimer -= Time.deltaTime;

            if (injureTimer <= 0)
            {
                injureTimer = 1 / injure;
                currentHealth -= 1;

                CheckHealth();
            }
        }
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
        if (!freeze && armCount != 0 && !isShielding && attackTimer <= 0 && Vector2.Distance((Vector2)transform.position, input.targetInput) < meleeRadius)
        {
            attackTimer = attackDelay;
            AttackLimbs(input.targetInput);
            AttackArea(input.targetInput, damage, false);
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

        if (freeze)
            return;

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
