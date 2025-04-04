using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DoubtReactions : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<DoubtSwapCardsGA>(DoubtReaction, ReactionTiming.POST);
		ActionSystem.SubscribeReaction<DoubtSwapCardsGA>(FlipBeforeDoubt, ReactionTiming.PRE);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<DoubtSwapCardsGA>(DoubtReaction, ReactionTiming.POST);
		ActionSystem.UnsubscribeReaction<DoubtSwapCardsGA>(FlipBeforeDoubt, ReactionTiming.PRE);
	}

	private void FlipBeforeDoubt(DoubtSwapCardsGA ga)
	{
		CardSystem cardSystem = FindFirstObjectByType<CardSystem>();

		VisualCardsHandler handler = ga.IsPlayer
			? GameObject.FindGameObjectWithTag("PlayerVisualHandler")?.GetComponent<VisualCardsHandler>()
			: GameObject.FindGameObjectWithTag("OpponentVisualHandler")?.GetComponent<VisualCardsHandler>();

		if (handler == null) return;

		HorizontalCardHolder hand = ga.IsPlayer
			? cardSystem.PlayerCardHolder
			: cardSystem.OpponentCardHolder;

		List<Card> handCards = new();
		foreach (Transform child in handler.transform)
		{
			CardDisplay display = child.GetComponent<CardDisplay>();
			if (display != null && display.OwnerCard != null && hand.cards.Contains(display.OwnerCard))
				handCards.Add(display.OwnerCard);
		}

		List<Card> pool = new(handCards);
		ga.cardsToSwap = new List<Card>();

		for (int i = 0; i < ga.Amount && pool.Count > 0; i++)
		{
			int index = Random.Range(0, pool.Count);
			Card cardToFlip = pool[index];
			pool.RemoveAt(index);

			ActionSystem.Instance.AddReaction(new FlipCardGA(cardToFlip, 0.2f));
			ga.cardsToSwap.Add(cardToFlip);
		}
	}

	private void DoubtReaction(DoubtSwapCardsGA ga)
	{
		if (ga.cardsToSwap == null || ga.cardsToSwap.Count == 0)
			return;

		CardSystem cardSystem = FindFirstObjectByType<CardSystem>();
		DeckSystem deck = FindFirstObjectByType<DeckSystem>();

		HorizontalCardHolder hand = ga.IsPlayer
			? cardSystem.PlayerCardHolder
			: cardSystem.OpponentCardHolder;

		if (ga.IsUltimate)
		{
			List<CardData> sourceDeck = ga.IsPlayer ? deck.playerDeck : deck.opponentDeck;
			int availableTens = sourceDeck.FindAll(c => c.cardValue == 10).Count;

			if (availableTens < ga.cardsToSwap.Count)
			{
				Debug.LogWarning("[DoubtReactions] Efeito Ultimate cancelado: N�o h� cartas 10 suficientes no deck.");
				return;
			}
		}

		foreach (Card card in ga.cardsToSwap)
		{
			GameObject discard = GameObject.FindWithTag("DiscardPoint");
			if (discard == null)
				continue;

			Vector3 targetPosition = discard.transform.position;

			Sequence seq = DOTween.Sequence();
			seq.Append(card.transform.DOMove(targetPosition, 0.2f).SetEase(Ease.InOutQuad));
			seq.OnComplete(() =>
			{
				ActionSystem.Instance.AddReaction(new DestroyCardGA(card));
			});
		}

		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Medium));

		var drawAction = ga.IsUltimate
			? new DrawCardGA(hand, ga.cardsToSwap.Count, 10)
			: new DrawCardGA(hand, ga.cardsToSwap.Count);

		ActionSystem.Instance.AddReaction(drawAction);
		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Medium));
	}
}
