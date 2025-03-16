using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using AxolotlProductions;


public class CardDisplay : MonoBehaviour
{
	public Card cardData;

	public Image cardImage;
	public TMP_Text valueText;

	private void Start() {
		UpdateCardDisplay();
	}

	public void UpdateCardDisplay() {
		valueText.text = cardData.cardValue.ToString();
	}
}
