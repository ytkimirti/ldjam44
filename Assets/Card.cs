using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{

    public bool isBuy;
    public string type;
    public float cost;
    public BuyMenu menu;

    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = cost.ToString();
        print("What het");
    }

    void Update()
    {

    }

    public void Clicked()
    {
        menu.ThingClicked(this);
    }
}
