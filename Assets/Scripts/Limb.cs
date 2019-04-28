using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Limb : MonoBehaviour
{

    public string limbType;
    public bool isRight;

    [Header("References")]
    public float jointLength;
    public Transform joint0;
    public Transform joint1;
    public SpriteRenderer joint0Sprite;
    public SpriteRenderer joint1Sprite;
    public SpriteRenderer feetSprite;
    [Space]
    public GameObject knifeGO;
    public GameObject feetGO;
    public GameObject gunGO;
    [Space]
    public GameObject bulletPrefab;

    [Header("Walking")]
    public bool isWalking;
    [Space]

    public Transform target;

    public float randomAmount;
    public float targetRadius;
    public float tweenSpeed;
    public float tweenHeight;

    [Header("Arms")]

    public bool isArm;
    public bool isKnife;
    public float armHeight;
    public float lerpSpeed;
    public float attackSpeed;

    [Header("Guns")]

    public bool isGun;

    [HideInInspector]
    public CharacterInput input;

    bool isTweening;
    Vector2 defPos;
    Vector2 currPos;

    void Awake()
    {
        isRight = transform.localPosition.x > 0;

        if (!isRight)
        {
            target.localPosition = new Vector3(-target.localPosition.x, target.localPosition.y, 0);
        }
    }

    public void SetColor(Color mainCol, Color secondCol)
    {
        joint0Sprite.color = secondCol;
        joint1Sprite.color = mainCol;
        feetSprite.color = mainCol;
    }

    void Start()
    {
        input = GetComponentInParent<CharacterInput>();

        ChangeType(limbType);

        if (isArm)
        {
            target.localPosition = new Vector3(0, armHeight, 0);

            randomAmount /= 4;
        }

        defPos = (Vector2)target.localPosition;

        if (Application.isPlaying)
            target.localPosition = target.localPosition + (Vector3)(Random.insideUnitCircle * randomAmount);

        currPos = defPos + (Vector2)transform.position;

        if (GetComponent<SortingGroup>())
            GetComponent<SortingGroup>().sortingOrder = Mathf.RoundToInt(transform.localPosition.y * -10);
    }

    void ChangeType(string type)
    {
        knifeGO.SetActive(false);
        feetGO.SetActive(false);
        gunGO.SetActive(false);

        isGun = false;
        isKnife = false;

        switch (type)
        {
            case "arm":
                isArm = true;
                isWalking = false;
                knifeGO.SetActive(true);
                break;
            case "arm_knife":
                isArm = true;
                isWalking = false;
                isKnife = true;
                knifeGO.SetActive(true);
                break;

            case "arm_gun":
                isArm = true;
                isWalking = false;
                isGun = true;
                gunGO.SetActive(true);
                break;
            case "leg":
                isArm = false;
                isWalking = true;
                feetGO.SetActive(true);
                break;
            default:
                isArm = false;
                isWalking = false;
                break;
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            if (isWalking)
            {
                feetGO.transform.eulerAngles = new Vector3(0, 0, 0);

                if (!isTweening && Vector2.Distance(currPos, (Vector2)transform.position + defPos) > targetRadius)
                {
                    TweenTarget();
                }

                if (isTweening)
                {
                    currPos = target.position;
                }
                else
                {
                    target.position = currPos;
                }
            }
            else if (isArm)
            {
                Vector2 mouseExtra = Vector2.ClampMagnitude((input.targetInput - (Vector2)target.position) / 4, jointLength * 1.9f);

                isRight = mouseExtra.x > 0;

                target.position = Vector2.Lerp(target.position, defPos + (Vector2)transform.position + mouseExtra, lerpSpeed * Time.deltaTime);
            }
        }
        else
        {

        }

        MoveLimb(target.position);
    }

    public void AttackLimb(Vector2 pos)
    {
        Vector2 movePos = (pos - (Vector2)transform.position).normalized * 1.9f;

        target.position = (Vector2)transform.position + movePos;
    }

    void TweenTarget()
    {
        isTweening = true;

        target.DOMove((Vector2)transform.position + defPos + Random.insideUnitCircle * randomAmount * 0.8f, tweenSpeed).OnComplete(OnTweenComplete);
    }

    void OnTweenComplete()
    {
        isTweening = false;

        currPos = target.position;

        ParticleManager.main.play(target.position, Vector3.zero, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(currPos, targetRadius);

        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere((Vector2)transform.position + defPos, 0.06f);
    }

    public Vector2 CalculateLimb(Vector2 pos)
    {
        float jointAngle0;
        float jointAngle1;

        float lengthDouble = jointLength * jointLength;

        float length2 = Vector2.Distance(joint0.position, pos);

        // Angle from Joint0 and Target
        Vector2 diff = pos - (Vector2)joint0.position;

        diff.y *= isRight ? -1 : 1;

        float atan = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // Is the target reachable?
        // If not, we stretch as far as possible
        if (jointLength * 2 < length2)
        {
            jointAngle0 = atan;
            jointAngle1 = 0f;
        }
        else
        {
            float cosAngle0 = ((length2 * length2) + (lengthDouble) - (lengthDouble)) / (2 * length2 * jointLength);
            float angle0 = Mathf.Acos(cosAngle0) * Mathf.Rad2Deg;

            float cosAngle1 = ((lengthDouble) + (lengthDouble) - (length2 * length2)) / (2 * lengthDouble);
            float angle1 = Mathf.Acos(cosAngle1) * Mathf.Rad2Deg;

            // So they work in Unity reference frame

            if (isRight)
            {
                jointAngle0 = (atan - angle0) * -1;
                jointAngle1 = (180f - angle1) * -1;
            }
            else
            {
                jointAngle0 = atan - angle0;
                jointAngle1 = 180f - angle1;
            }
        }

        return new Vector2(jointAngle0, jointAngle1);
    }

    public void MoveLimb(Vector2 pos)
    {
        Vector2 angles = CalculateLimb(pos);

        joint0.localEulerAngles = new Vector3(0, 0, angles.x);
        joint1.localEulerAngles = new Vector3(0, 0, angles.y);
    }


}
