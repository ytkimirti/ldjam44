using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public int currentHealth;
    int textHealth;
    [Space]
    public TextMeshPro textMesh;
    public float delay;
    float timer;

    void Start()
    {
        textHealth = currentHealth;
        textMesh.text = textHealth.ToString();
    }


    public void Die()
    {
        transform.DOScale(Vector3.zero, 0.5f).SetDelay(delay).OnComplete(Despawn);
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (textHealth != currentHealth && timer <= 0)
        {
            timer = delay / Mathf.Abs(currentHealth - textHealth);
            textHealth += currentHealth - textHealth > 0 ? 1 : -1;
            textMesh.transform.DOPunchScale(Vector3.up * 0.4f, timer / 2);
            textMesh.text = textHealth.ToString();
        }
    }
}
