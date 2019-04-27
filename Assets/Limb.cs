using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Limb : MonoBehaviour
{

    [Header("References")]
    public float jointLength;
    public Transform joint0;
    public Transform joint1;

    [Header("Walking")]
    public bool isWalking;
    [Space]

    public Transform target;

    public float randomAmount;
    public float targetRadius;
    public float tweenSpeed;
    public float tweenHeight;

    bool isTweening;
    Vector2 defPos;
    Vector2 currPos;

    void Start()
    {
        defPos = (Vector2)target.localPosition;
        target.localPosition = target.localPosition + (Vector3)(Random.insideUnitCircle * randomAmount);
        currPos = defPos + (Vector2)transform.position;
    }

    void Update()
    {


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

        MoveLimb(target.position);
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

        diff.y *= diff.x > 0 ? -1 : 1;

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

            if (diff.x > 0)
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
