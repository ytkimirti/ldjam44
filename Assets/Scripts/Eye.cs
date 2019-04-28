using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Eye : MonoBehaviour
{
    public Transform eye_white;
    public Transform eye_black;
    public SpriteRenderer eyelash;
    [Space]
    public float randomEyeTime;
    float eyeTimer;


    public float eyeCloseSpeed;
    public float moveAmount;
    public float blackMoveAmount;

    CharacterInput input;

    void Start()
    {
        input = GetComponentInParent<CharacterInput>();

        eyeTimer = Random.Range(1, randomEyeTime);
    }

    public void SetColor(Color col)
    {
        eyelash.color = col;
    }

    public void Wink()
    {
        eyelash.gameObject.SetActive(true);
        eyelash.DORewind();

        eyelash.DOKill();

        Sequence winkSeq = DOTween.Sequence();

        winkSeq.Append(eyelash.transform.DOLocalMoveY(0, eyeCloseSpeed));

        winkSeq.AppendInterval(eyeCloseSpeed);

        winkSeq.Append(eyelash.transform.DOLocalMoveY(1, eyeCloseSpeed)).OnComplete(DisableEyelash);

        winkSeq.Play();
    }

    void DisableEyelash()
    {
        eyelash.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector2 localMovement = input.targetInput - (Vector2)transform.position;

        localMovement.Normalize();

        eye_white.transform.localPosition = Vector2.ClampMagnitude(localMovement / 5, moveAmount);

        eye_black.transform.localPosition = Vector2.ClampMagnitude(localMovement / 5, blackMoveAmount);

        eyeTimer -= Time.deltaTime;

        if (eyeTimer <= 0)
        {
            Wink();
            eyeTimer = Random.Range(0.1f, randomEyeTime);
        }
    }
}
