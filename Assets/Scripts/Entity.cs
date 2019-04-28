using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Health
{
    [Header("Attacking")]
    public float meleeRadius;
    public float searchArea;
    public LayerMask attackLayer;


    public void AddToList()
    {
        GameManager.main.entityList.Add(this);
    }

    void Start()
    {
        AddToList();
        SpawnHealthBar();
    }

    public override void Die()
    {
        GameManager.main.entityList.Remove(this);
        base.Die();
    }

    public virtual void OnAttackButtonPressed()
    {

    }

    public virtual void AddGore(Vector2 attackPos)
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
