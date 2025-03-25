using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerHand;
	[SerializeField] private HorizontalCardHolder opponentHand;
	public List<CardData> playerDeck = new();
	public List<CardData> opponentDeck = new();
	[SerializeField] private GameObject cardSlotPrefab;
	[SerializeField] private int cardsToDraw = 8;

	private void Start()
	{
		ActionSystem.Instance.Perform(new GenerateDecksGA());
		ActionSystem.Instance.Perform(new ShuffleDeckGA(playerDeck));
		ActionSystem.Instance.Perform(new ShuffleDeckGA(opponentDeck));
		SpawnInitialCards(playerHand, cardsToDraw);
		SpawnInitialCards(opponentHand, cardsToDraw);
	}

	private void SpawnInitialCards(HorizontalCardHolder hand, int count)
	{
		if (hand.cards == null)
			hand.cards = new List<Card>();

		for (int i = 0; i < count; i++)
		{
			GameObject slotGO = Instantiate(cardSlotPrefab, hand.transform);
			Card card = slotGO.GetComponentInChildren<Card>();
			card.isPlayerCard = hand.IsPlayerCardHolder;

			hand.cards.Add(card);

			card.PointerEnterEvent.AddListener(hand.CardPointerEnter);
			card.PointerExitEvent.AddListener(hand.CardPointerExit);
			card.BeginDragEvent.AddListener(hand.BeginDrag);
			card.EndDragEvent.AddListener(hand.EndDrag);

			card.name = hand.cards.Count.ToString();
		}

		for (int i = 0; i < hand.cards.Count; i++)
		{
			if (hand.cards[i].cardVisual != null)
				hand.cards[i].cardVisual.UpdateIndex(hand.transform.childCount);
		}
	}
}
