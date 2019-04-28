using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuyMenu : MonoBehaviour
{
    Spider player;
    float defHeight;

    public float openSpeed;
    public float closeSpeed;
    public Ease ease;
    float targetTime = 1;

    public Transform dieInfoText;

    public float lerpSpeed;

    public bool isOn;

    void Start()
    {
        targetTime = 1;
        player = GameManager.main.player;
        defHeight = transform.localPosition.y;
    }

    void Update()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTime, Time.deltaTime * lerpSpeed);

        if (targetTime == 1 && Time.timeScale > 0.98f)
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOn)
                Close();
            else
                Open();
        }

        if (player.currentHealth < 30 && !isOn && !GameManager.main.isGameEnded)
        {

            if (dieInfoText.localScale.x == 0)
            {
                targetTime = 0.2f;
                dieInfoText.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutElastic);
            }
        }
        else
        {
            if (dieInfoText.localScale.x == 1)
            {
                dieInfoText.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutElastic);
            }
        }

        if (GameManager.main.isGameEnded)
            Time.timeScale = 1;
    }

    public void Open()
    {
        if (player.freeze)
            return;

        if (isOn)
            return;

        transform.DOKill();

        transform.DOLocalMoveY(0, openSpeed).SetEase(ease);
        targetTime = 0.02f;

        isOn = true;
    }

    public void Close()
    {
        if (!isOn)
            return;

        transform.DOKill();

        isOn = false;

        transform.DOLocalMoveY(defHeight, closeSpeed).SetEase(ease);

        targetTime = 1;
    }

    public void Buy(string type, float cost)
    {
        if (player.currentHealth < cost)
            return;

        switch (type)
        {
            case "leg":
                player.legCount++;
                break;
            case "eye":
                player.eyeCount++;
                break;
            case "arm":
                player.armCount++;
                break;
            default:
                player.legCount++;
                break;
        }

        GameManager.main.DecreaseHP(cost);

        Close();
    }

    public void Sell(string type, float cost)
    {
        int thingCount = 0;

        switch (type)
        {
            case "leg":
                thingCount = player.legCount;
                break;
            case "eye":
                thingCount = player.eyeCount;
                break;
            case "arm":
                thingCount = player.armCount;
                break;
        }

        if (thingCount <= 0)
            return;

        switch (type)
        {
            case "leg":
                player.legCount--;
                break;
            case "eye":
                player.eyeCount--;
                break;
            case "arm":
                player.armCount--;
                break;
            default:
                player.legCount--;
                break;
        }

        GameManager.main.IncreaseHP(cost);

        Close();
    }

    public void Cure(float cost)
    {
        if (player.currentHealth < cost)
            return;

        player.ClearInjures();

        GameManager.main.DecreaseHP(cost);

        Close();
    }

    public void ThingClicked(Card card)
    {
        print("cliked");

        if (card.type == "cure")
        {
            Cure(card.cost);

            return;
        }

        if (card.isBuy)
        {
            Buy(card.type, card.cost);
        }
        else
        {
            Sell(card.type, card.cost);
        }

        player.UpdateParts();
        ParticleManager.main.play(player.transform.position, Vector2.zero, 2);
    }
}
