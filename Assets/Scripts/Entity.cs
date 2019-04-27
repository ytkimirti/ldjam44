using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Health
{
    [Header("Attacking")]
    public float meleeRadius;
    public float searchArea;
    public LayerMask attackLayer;

    void Start()
    {
        SpawnHealthBar();
    }

    public virtual void OnAttackButtonPressed()
    {

    }

    public virtual void AttackArea(Vector2 pos, float damage)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, searchArea, attackLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D col = colliders[i];

            if (!col || col.gameObject == this.gameObject)
                return;

            Health hp = col.gameObject.GetComponent<Health>();

            if (hp)
            {
                hp.GetDamage(damage);
            }
        }
    }
}
