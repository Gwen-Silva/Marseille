using AxolotlProductions;
using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour {

	[Header("Deck Configuration")]
	[SerializeField] private int maxHandSize = 6;

	public List<Card> allCards = new List<Card>();
	private int currentIndex = 0;

	private void Start() {
		GenerateDeck();

		HandManager hand = FindFirstObjectByType<HandManager>();
		for (int i = 0; i < maxHandSize; i++) {
			DrawCard(hand);
		}
	}

	private void GenerateDeck() {
		allCards.Clear();

		foreach (CardEffect effect in System.Enum.GetValues(typeof(CardEffect))) {
			for (int value = 1; value <= 10; value++) {
				Card newCard = ScriptableObject.CreateInstance<Card>();
				newCard.Initialize(value, effect);
				allCards.Add(newCard);
			}
		}

		ShuffleDeck();
	}

	private void ShuffleDeck() {
		for (int i = allCards.Count - 1; i > 0; i--) {
			int randIndex = Random.Range(0, i + 1);
			(allCards[i], allCards[randIndex]) = (allCards[randIndex], allCards[i]);
		}
	}

	public void DrawCard(HandManager handManager) {
		if (handManager.cardsInHand.Count >= maxHandSize) {
			return;
		}

		if (allCards.Count == 0) return;

		Card nextCard = allCards[currentIndex];
		handManager.AddCardToHand(nextCard);
		currentIndex = (currentIndex + 1) % allCards.Count;
	}

	public int GetMaxHandSize() {
		return maxHandSize;
	}
}
