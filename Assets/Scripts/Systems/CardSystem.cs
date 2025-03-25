using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class CardSystem : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
		ActionSystem.AttachPerformer<SelectCardGA>(SelectCardPerformer);
		ActionSystem.AttachPerformer<DeselectCardGA>(DeselectCardPerformer);
		ActionSystem.AttachPerformer<SwapCardGA>(SwapCardPerformer);
		ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
		ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
		ActionSystem.AttachPerformer<FlipCardGA>(FlipCardPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<DrawCardGA>();
		ActionSystem.DetachPerformer<SelectCardGA>();
		ActionSystem.DetachPerformer<DeselectCardGA>();
		ActionSystem.DetachPerformer<SwapCardGA>();
		ActionSystem.DetachPerformer<DestroyCardGA>();
		ActionSystem.DetachPerformer<DrawCardGA>();
		ActionSystem.DetachPerformer<PlayCardGA>();
		ActionSystem.DetachPerformer<FlipCardGA>();
	}

	private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
	{
		DeckSystem deckSystem = FindFirstObjectByType<DeckSystem>();
		List<Card> spawnedCards = new();

		for (int i = 0; i < drawCardGA.amount; i++)
		{
			CardData data = drawCardGA.targetHolder.IsPlayerCardHolder
				? deckSystem.DrawFromPlayerDeck()
				: deckSystem.DrawFromOpponentDeck();

			if (data == null)
			{
				Debug.LogWarning("Tentou comprar carta de um baralho vazio.");
				break;
			}

			GameObject slotGO = Instantiate(drawCardGA.targetHolder.SlotPrefab, drawCardGA.targetHolder.transform);
			Card card = slotGO.GetComponentInChildren<Card>();
			if (card == null)
			{
				Debug.LogError("Slot prefab não contém componente Card.");
				continue;
			}

			card.isPlayerCard = drawCardGA.targetHolder.IsPlayerCardHolder;

			CardDisplay display = card.GetComponentInChildren<CardDisplay>();
			if (display != null)
			{
				display.cardData = data;
				display.UpdateCardDisplay();
			}

			drawCardGA.targetHolder.cards.Add(card);
			spawnedCards.Add(card);

			card.PointerEnterEvent.AddListener(drawCardGA.targetHolder.CardPointerEnter);
			card.PointerExitEvent.AddListener(drawCardGA.targetHolder.CardPointerExit);
			card.BeginDragEvent.AddListener(drawCardGA.targetHolder.BeginDrag);
			card.EndDragEvent.AddListener(drawCardGA.targetHolder.EndDrag);
			card.name = drawCardGA.targetHolder.cards.Count.ToString();

			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSecondsRealtime(0.1f);

		for (int i = 0; i < drawCardGA.targetHolder.cards.Count; i++)
		{
			if (drawCardGA.targetHolder.cards[i].cardVisual != null)
				drawCardGA.targetHolder.cards[i].cardVisual.UpdateIndex(drawCardGA.targetHolder.transform.childCount);
		}

		drawCardGA.spawnedCards = spawnedCards;
	}

	private IEnumerator PlayCardPerformer(PlayCardGA action)
	{
		yield return null;
	}

	private IEnumerator SelectCardPerformer(SelectCardGA selectCardGA)
	{
		selectCardGA.card.selected = !selectCardGA.card.selected;
		selectCardGA.card.SelectEvent.Invoke(selectCardGA.card, selectCardGA.card.selected);

		if (selectCardGA.card.selected)
			selectCardGA.card.transform.localPosition += (selectCardGA.card.cardVisual.transform.up * selectCardGA.card.selectionOffset);
		else
			selectCardGA.card.transform.localPosition = Vector3.zero;

		yield return null;
	}

	private IEnumerator DeselectCardPerformer(DeselectCardGA deselectCardGA)
	{
		if (deselectCardGA.card.selected)
		{
			deselectCardGA.card.selected = false;
			deselectCardGA.card.SelectEvent.Invoke(deselectCardGA.card, false);

			if (deselectCardGA.card.cardVisual != null)
				deselectCardGA.card.transform.localPosition = Vector3.zero;
		}
		else
		{
			deselectCardGA.card.transform.localPosition = Vector3.zero;
		}

		yield return null;
	}

	private IEnumerator SwapCardPerformer(SwapCardGA swapCardGA)
	{
		Transform sourceParent = swapCardGA.sourceCard.transform.parent;
		Transform targetParent = swapCardGA.targetCard.transform.parent;

		swapCardGA.targetCard.transform.SetParent(sourceParent);
		swapCardGA.targetCard.transform.localPosition = swapCardGA.targetCard.selected
			? new Vector3(0, swapCardGA.targetCard.selectionOffset, 0)
			: Vector3.zero;

		swapCardGA.sourceCard.transform.SetParent(targetParent);

		if (swapCardGA.targetCard.cardVisual != null)
		{
			bool swapIsRight = swapCardGA.targetCard.ParentIndex() > swapCardGA.sourceCard.ParentIndex();
			swapCardGA.targetCard.cardVisual.Swap(swapIsRight ? -1 : 1);
		}

		foreach (Card card in swapCardGA.parent.GetComponentsInChildren<Card>())
		{
			if (card.cardVisual != null)
				card.cardVisual.UpdateIndex(swapCardGA.parent.childCount);
		}

		yield return null;
	}

	private IEnumerator DestroyCardPerformer(DestroyCardGA destroyCardGA)
	{
		if (destroyCardGA.card != null)
		{
			HorizontalCardHolder holder = destroyCardGA.card.GetComponentInParent<HorizontalCardHolder>();
			if (holder != null)
				holder.cards.Remove(destroyCardGA.card);

			if (destroyCardGA.card.cardVisual != null)
			{
				DOTween.Kill(destroyCardGA.card.cardVisual.transform);
				Destroy(destroyCardGA.card.cardVisual.gameObject);
			}
			Destroy(destroyCardGA.card.transform.parent.gameObject);
		}
		yield return null;
	}

	private IEnumerator FlipCardPerformer(FlipCardGA flipCardGA)
	{
		CardVisual visual = flipCardGA.card.cardVisual;
		if (visual == null)
			yield break;

		visual.isFlipped = !visual.isFlipped;

		float angle = visual.isFlipped ? 180f : 0f;
		float duration = 0.25f;

		visual.FlipParent.transform.DOLocalRotate(new Vector3(0f, angle, 0f), duration);

		yield return new WaitForSeconds(duration);
	}
}