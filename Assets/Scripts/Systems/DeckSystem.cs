using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
	[Header("Deck Settings")]
	public List<CardData> allCardTemplates;
	public List<CardData> playerDeck = new();
	public List<CardData> opponentDeck = new();

	public int cardsPerSuit = 10;

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

	private void Awake()
	{
		ActionSystem.Instance.Perform(new GenerateDecksGA());
	}

	public CardData DrawFromPlayerDeck()
	{
		if (playerDeck.Count == 0) return null;

		CardData card = playerDeck[0];
		playerDeck.RemoveAt(0);
		return card;
	}

	public CardData DrawFromOpponentDeck()
	{
		if (opponentDeck.Count == 0) return null;

		CardData card = opponentDeck[0];
		opponentDeck.RemoveAt(0);
		return card;
	}

	private IEnumerator GenerateDecksPerformer(GenerateDecksGA action)
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

		ActionSystem.Instance.Perform(new ShuffleDeckGA(playerDeck));
		ActionSystem.Instance.Perform(new ShuffleDeckGA(opponentDeck));

		yield return null;
	}

	private IEnumerator ShuffleDeckPerformer(ShuffleDeckGA action)
	{
		List<CardData> deck = action.deck;
		for (int i = deck.Count - 1; i > 0; i--)
		{
			int rand = Random.Range(0, i + 1);
			(deck[i], deck[rand]) = (deck[rand], deck[i]);
		}
		yield return null;
	}
}
