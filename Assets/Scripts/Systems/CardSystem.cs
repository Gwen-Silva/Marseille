using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class CardSystem : MonoBehaviour
{
	[SerializeField] private Transform discardPoint;
	[SerializeField] private HorizontalCardHolder playerHand;
	[SerializeField] private HorizontalCardHolder opponentHand;
	public HorizontalCardHolder PlayerCardHolder => playerHand;
	public HorizontalCardHolder OpponentCardHolder => opponentHand;

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
		ActionSystem.AttachPerformer<SelectCardGA>(SelectCardPerformer);
		ActionSystem.AttachPerformer<DeselectCardGA>(DeselectCardPerformer);
		ActionSystem.AttachPerformer<SwapCardGA>(SwapCardPerformer);
		ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
		ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
		ActionSystem.AttachPerformer<FlipCardGA>(FlipCardPerformer);
		ActionSystem.AttachPerformer<DrawInitialCardsGA>(DrawInitialCardsPerformer);
		ActionSystem.AttachPerformer<ClearBoardGA>(ClearBoardPerformer);
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
		ActionSystem.DetachPerformer<DrawInitialCardsGA>();
		ActionSystem.DetachPerformer<ClearBoardGA>();
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

			GameObject slotGO = Instantiate(drawCardGA.targetHolder.SlotPrefab, drawCardGA.targetHolder.transform);
			Card card = slotGO.GetComponentInChildren<Card>();

			card.isPlayerCard = drawCardGA.targetHolder.IsPlayerCardHolder;
			card.cardData = data;

			CardDisplay display = card.GetComponentInChildren<CardDisplay>();

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

	private IEnumerator DrawInitialCardsPerformer(DrawInitialCardsGA ga)
	{
		var drawPlayer = new DrawCardGA(ga.playerHand, ga.amount);
		var drawOpponent = new DrawCardGA(ga.opponentHand, ga.amount);

		ActionSystem.Instance.AddReaction(drawPlayer);
		ActionSystem.Instance.AddReaction(drawOpponent);

		ActionSystem.Instance.AddReaction(new StartGameGA());

		yield return null;
	}

	private IEnumerator PlayCardPerformer(PlayCardGA action)
	{
		CardDisplay cardDisplay = action.Card;
		Card card = cardDisplay.OwnerCard;

		card.DisableInteraction();

		cardDisplay.transform.position = action.TargetSlot.transform.position;
		cardDisplay.transform.rotation = action.TargetSlot.transform.rotation;
		Vector3 boardScale = new Vector3(0.85f, 0.85f, 0.85f);
		cardDisplay.transform.DOScale(boardScale, 0.2f).SetEase(Ease.OutQuad);

		if (action.IsValueSlot)
		{
			cardDisplay.ChangeToValueSprite();
		}

		HorizontalCardHolder holder = card.GetComponentInParent<HorizontalCardHolder>();
		if (holder != null)
		{
			holder.cards.Remove(card);
		}
		Destroy(card.transform.parent.gameObject);

		card.transform.SetParent(action.TargetSlot.transform);
		card.transform.localPosition = Vector3.zero;

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
		Card card = destroyCardGA.card;

		if (card == null)
			yield break;

		Debug.Log($"[DestroyCardPerformer] Iniciando destruição da carta: {card.name}");

		HorizontalCardHolder holder = card.GetComponentInParent<HorizontalCardHolder>();
		if (holder != null)
		{
			holder.cards.Remove(card);
		}

		if (card.cardVisual != null)
		{
			DOTween.Kill(card.cardVisual.transform);
			Destroy(card.cardVisual.gameObject);
		}

		Transform slot = card.transform.parent;
		DOTween.Kill(slot);
		Destroy(slot.gameObject);

		yield return null;
	}

	private IEnumerator FlipCardPerformer(FlipCardGA flipCardGA)
	{
		CardVisual visual = flipCardGA.card.cardVisual;
		if (visual == null)
			yield break;

		visual.isFlipped = !visual.isFlipped;

		float angle = visual.isFlipped ? 180f : 0f;
		float duration = flipCardGA.duration;

		visual.FlipParent.transform.DOLocalRotate(new Vector3(0f, angle, 0f), duration);

		yield return new WaitForSeconds(duration);
	}

	private IEnumerator ClearBoardPerformer(ClearBoardGA action)
	{
		Debug.Log("[ClearBoardPerformer] Limpando cartas do board...");

		var boardSlots = GameObject.FindObjectsByType<CardDropZone>(FindObjectsSortMode.None)
			.Where(slot => slot.CompareTag("BoardSlot"))
			.ToList();

		foreach (var slot in boardSlots)
		{
			if (slot.transform.childCount > 0)
			{
				Card card = slot.GetComponentInChildren<Card>();
				if (card != null)
				{
					card.DisableInteraction();

					Sequence discardSequence = DOTween.Sequence();

					discardSequence.Append(card.transform.DOMove(discardPoint.position, 0.2f).SetEase(Ease.InOutQuad));

					discardSequence.OnComplete(() =>
					{
						if (card.cardVisual != null)
							Destroy(card.cardVisual.gameObject);

						Destroy(card.gameObject);
					});

					yield return discardSequence.WaitForCompletion();

					CardDropZone dropZone = slot.GetComponent<CardDropZone>();
					if (dropZone != null)
						dropZone.ResetSlot();
				}
			}
		}

		yield return null;
	}
}