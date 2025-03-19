using AxolotlProductions;
using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour {
	[Header("Deck Configuration")]
	[SerializeField] private int maxHandSize = 6;

	public List<Card> playerDeck = new List<Card>();
	public List<Card> opponentDeck = new List<Card>();

	private int playerIndex = 0;
	private int opponentIndex = 0;

	private void Start() {
		GenerateDecks();

		BaseHandManager playerHand = FindFirstObjectByType<HandManager>();
		BaseHandManager opponentHand = FindFirstObjectByType<OpponentHandManager>();

		for (int i = 0; i < maxHandSize; i++) {
			DrawCard(playerHand, true);
			DrawCard(opponentHand, false);
		}
	}

	private void GenerateDecks() {
		playerDeck.Clear();
		opponentDeck.Clear();

		foreach (CardEffect effect in System.Enum.GetValues(typeof(CardEffect))) {
			for (int value = 1; value <= 10; value++) {
				Card playerCard = ScriptableObject.CreateInstance<Card>();
				playerCard.Initialize(value, effect);
				playerDeck.Add(playerCard);

				Card opponentCard = ScriptableObject.CreateInstance<Card>();
				opponentCard.Initialize(value, effect);
				opponentDeck.Add(opponentCard);
			}
		}

		ShuffleDeck(playerDeck);
		ShuffleDeck(opponentDeck);
	}

	private void ShuffleDeck(List<Card> deck) {
		for (int i = deck.Count - 1; i > 0; i--) {
			int randIndex = Random.Range(0, i + 1);
			(deck[i], deck[randIndex]) = (deck[randIndex], deck[i]);
		}
	}

	public void DrawCard(BaseHandManager handManager, bool isPlayer) {
		List<Card> activeDeck = isPlayer ? playerDeck : opponentDeck;
		int activeIndex = isPlayer ? playerIndex : opponentIndex;

		if (handManager.cardsInHand.Count >= maxHandSize) {
			return;
		}

		if (activeDeck.Count == 0) return;

		Card nextCard = activeDeck[activeIndex];
		handManager.AddCardToHand(nextCard);

		if (isPlayer) {
			playerIndex = (playerIndex + 1) % activeDeck.Count;
		}
		else {
			opponentIndex = (opponentIndex + 1) % activeDeck.Count;
		}
	}

	public int GetMaxHandSize() {
		return maxHandSize;
	}
}
