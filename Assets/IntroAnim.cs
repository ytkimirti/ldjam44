using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class IntroAnim : MonoBehaviour
{

    public TextMeshProUGUI text;
    public Transform newPopup;

    public string[] texts;
    public float delay;

    public float firstDelay;

    void Start()
    {
        StartCoroutine(anim());
    }

    void Update()
    {

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

        yield return new WaitForSeconds(4);

        newPopup.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
    }
}
