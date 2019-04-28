using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    public bool isBuy;
    public string type;
    public float cost;
    public BuyMenu menu;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Clicked()
    {
        menu.ThingClicked(this);
    }
}
