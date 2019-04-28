using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{

    public bool spawnHealthBar = true;
    public float healthBarHeight = 1.5f;
    [Space]
    public bool hasMaxHealth;
    public float maxHealth;
    [Space]
    public float currentHealth;
    HealthBar healthBar;

    void Start()
    {
        SpawnHealthBar();
    }

    public void SpawnHealthBar()
    {
        if (!spawnHealthBar)
            return;

        GameObject healthBarGO = Instantiate(GameManager.main.healthBarPrefab, transform.position + Vector3.up * healthBarHeight, Quaternion.identity, GameManager.main.canvasParent);
        healthBar = healthBarGO.GetComponent<HealthBar>();
        healthBar.currentHealth = Mathf.RoundToInt(currentHealth);
        healthBar.offsetHeight = healthBarHeight;
        healthBar.target = this.transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * healthBarHeight, new Vector3(1, 0.1f, 0));
    }

    public void UpdateHealthBar()
    {
        if (!spawnHealthBar)
            return;

        if (!healthBar)
            SpawnHealthBar();

        healthBar.currentHealth = Mathf.RoundToInt(currentHealth);
    }

    public void DestroyHealthBar()
    {
        healthBar.transform.parent = null;
        healthBar.currentHealth = 0;
        healthBar.Die();
    }

    public virtual void GetDamage(float amount)
    {
        print(name + " got " + amount + "damage");
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        UpdateHealthBar();
    }

    public virtual void Die()
    {
        DestroyHealthBar();
        Destroy(gameObject);
    }
}
