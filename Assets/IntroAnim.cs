using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class IntroAnim : MonoBehaviour
{

    public GameObject hand;
    public static IntroAnim main;
    public TextMeshProUGUI text;
    public Transform newPopup;
    public Transform skipButton;

    public string[] texts;
    public float delay;

    public float firstDelay;

    public bool isIntroDone = false;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        StartCoroutine("anim");
    }

    void Update()
    {

    }

    public void SkipIntro()
    {
        print("Skipping intro");
        StopCoroutine("anim");
        GameManager.main.player.SkipIntro();

        skipButton.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutExpo);
        newPopup.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutExpo);
        text.transform.parent.DOScale(Vector2.zero, 0.5f).SetEase(Ease.InOutExpo);
        GetComponent<Animator>().enabled = false;
        hand.SetActive(false);
    }

    IEnumerator anim()
    {
        yield return new WaitForSeconds(firstDelay);

        for (int i = 0; i < texts.Length; i++)
        {
            text.transform.DOPunchScale(Vector3.one * 0.25f, 0.5f);
            text.text = texts[i];

            yield return new WaitForSeconds(delay);
        }

        text.transform.parent.DOScale(Vector2.zero, 1f);

        yield return new WaitForSeconds(1);

        newPopup.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutElastic);

        yield return new WaitForSeconds(8);

        newPopup.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
    }
}
