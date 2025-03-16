using AxolotlProductions;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DeckManager : MonoBehaviour
{
   public List<Card> allCards = new List<Card>();

	private int currentIndex = 0;

	private void Start()
	{
		Card[] cards = Resources.LoadAll<Card>("Cards");

		allCards.AddRange(cards);

		HandManager hand = Object.FindFirstObjectByType<HandManager>();
		for (int i = 0; i < 6; i++) {
			DrawCard(hand);
		}
	}

	public void DrawCard(HandManager handManager) {
		if (allCards.Count == 0)
			return;

		Card nextCard = allCards[currentIndex];
		handManager.AddCardToHand(nextCard);
		currentIndex = (currentIndex + 1) % allCards.Count;
	}
}
