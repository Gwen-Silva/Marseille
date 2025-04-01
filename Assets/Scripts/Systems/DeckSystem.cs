using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
	#region Fields

	[Header("Deck Settings")]
	public List<CardData> allCardTemplates;
	public List<CardData> playerDeck = new();
	public List<CardData> opponentDeck = new();

	public int cardsPerSuit = 10;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<GenerateDecksGA>(GenerateDecksPerformer);
		ActionSystem.AttachPerformer<ShuffleDeckGA>(ShuffleDeckPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<GenerateDecksGA>();
		ActionSystem.DetachPerformer<ShuffleDeckGA>();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Draws and removes the top card from the player deck.
	/// </summary>
	public CardData DrawFromPlayerDeck()
	{
		if (playerDeck.Count == 0) return null;

		CardData card = playerDeck[0];
		playerDeck.RemoveAt(0);
		return card;
	}

	/// <summary>
	/// Draws and removes the top card from the opponent deck.
	/// </summary>
	public CardData DrawFromOpponentDeck()
	{
		if (opponentDeck.Count == 0) return null;

		CardData card = opponentDeck[0];
		opponentDeck.RemoveAt(0);
		return card;
	}

	#endregion

	#region Performers

	/// <summary>
	/// Instantiates the decks for both players based on template and cards per suit.
	/// </summary>
	private IEnumerator GenerateDecksPerformer(GenerateDecksGA ga)
	{
		playerDeck.Clear();
		opponentDeck.Clear();

		foreach (CardData template in allCardTemplates)
		{
			for (int i = 1; i <= cardsPerSuit; i++)
			{
				CardData playerCard = Instantiate(template);
				playerCard.cardValue = i;
				playerDeck.Add(playerCard);

				CardData opponentCard = Instantiate(template);
				opponentCard.cardValue = i;
				opponentDeck.Add(opponentCard);
			}
		}

		yield return null;
	}

	/// <summary>
	/// Shuffles the given deck using Fisher-Yates algorithm.
	/// </summary>
	private IEnumerator ShuffleDeckPerformer(ShuffleDeckGA ga)
	{
		List<CardData> deck = ga.deck;
		for (int i = deck.Count - 1; i > 0; i--)
		{
			int randIndex = Random.Range(0, i + 1);
			(deck[i], deck[randIndex]) = (deck[randIndex], deck[i]);
		}

		yield return null;
	}
}
#endregion