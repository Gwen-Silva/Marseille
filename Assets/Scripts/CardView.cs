using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using AxolotlProductions;
using System;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text value;
    public Card card;
    public void Setup(Card card) {
        this.card = card;
        value.text = card.Value.ToString();
    }
}
