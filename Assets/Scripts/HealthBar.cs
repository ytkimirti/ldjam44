using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public float screenOffset;
    public float offsetHeight;
    public int currentHealth;
    int textHealth;
    [Space]
    public TextMeshProUGUI textMesh;
    public Transform bloodSprite;
    public float delay;
    float timer;
    public Transform target;

    Camera cam;
    public bool isDed;

    void Start()
    {
        cam = CameraController.main.cam;
        textHealth = currentHealth;
        textMesh.text = textHealth.ToString();
    }


    public void Die()
    {
        if (isDed)
            return;

        isDed = true;
        transform.DOScale(Vector3.zero, 0.5f).SetDelay(delay).OnComplete(Despawn);

    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (target)
            transform.position = cam.WorldToScreenPoint(target.position + Vector3.up * offsetHeight) + Vector3.up * screenOffset;
        else
            Die();

        timer -= Time.deltaTime;

        if (textHealth != currentHealth && timer <= 0)
        {
            timer = delay / Mathf.Abs(currentHealth - textHealth);
            textHealth += currentHealth - textHealth > 0 ? 1 : -1;
            bloodSprite.DOPunchScale(Vector3.up * 0.4f, timer / 2);
            textMesh.text = textHealth.ToString();
        }
    }
}
