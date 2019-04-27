using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public bool hasMaxHealth;
    public float maxHealth;
    [Space]
    public float currentHealth;

    void Start()
    {

    }

    public virtual void GetDamage(float amount)
    {
        print(name + " got " + amount + "damage");
    }
}
