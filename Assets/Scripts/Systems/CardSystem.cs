using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardSystem : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder horizontalCardHolder;

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
		ActionSystem.AttachPerformer<SelectCardGA>(SelectCardPerformer);
		ActionSystem.AttachPerformer<DeselectCardGA>(DeselectCardPerformer);
		ActionSystem.AttachPerformer<SwapCardGA>(SwapCardPerformer);
		ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
		ActionSystem.AttachPerformer<SpawnCardGA>(SpawnCardPerformer);
		ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<DrawCardGA>();
		ActionSystem.DetachPerformer<SelectCardGA>();
		ActionSystem.DetachPerformer<DeselectCardGA>();
		ActionSystem.DetachPerformer<SwapCardGA>();
		ActionSystem.DetachPerformer<DestroyCardGA>();
		ActionSystem.DetachPerformer<SpawnCardGA>();
		ActionSystem.DetachPerformer<PlayCardGA>();
	}

	private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
	{
		drawCardGA.targetHolder.SpawnCards(drawCardGA.Amount);
		yield return null;
	}

	private IEnumerator PlayCardPerformer(PlayCardGA action)
	{
		yield return null;
	}

	private IEnumerator SelectCardPerformer(SelectCardGA action)
	{
		action.card.selected = !action.card.selected;
		action.card.SelectEvent.Invoke(action.card, action.card.selected);

		if (action.card.selected)
			action.card.transform.localPosition += (action.card.cardVisual.transform.up * action.card.selectionOffset);
		else
			action.card.transform.localPosition = Vector3.zero;

		yield return null;
	}

	private IEnumerator DeselectCardPerformer(DeselectCardGA action)
	{
		if (action.card.selected)
		{
			action.card.selected = false;
			action.card.SelectEvent.Invoke(action.card, false);

			if (action.card.cardVisual != null)
				action.card.transform.localPosition = Vector3.zero;
		}
		else
		{
			action.card.transform.localPosition = Vector3.zero;
		}

		yield return null;
	}

	private IEnumerator SwapCardPerformer(SwapCardGA action)
	{
		Transform sourceParent = action.sourceCard.transform.parent;
		Transform targetParent = action.targetCard.transform.parent;

		action.targetCard.transform.SetParent(sourceParent);
		action.targetCard.transform.localPosition = action.targetCard.selected
			? new Vector3(0, action.targetCard.selectionOffset, 0)
			: Vector3.zero;

		action.sourceCard.transform.SetParent(targetParent);

		if (action.targetCard.cardVisual != null)
		{
			bool swapIsRight = action.targetCard.ParentIndex() > action.sourceCard.ParentIndex();
			action.targetCard.cardVisual.Swap(swapIsRight ? -1 : 1);
		}

		foreach (Card card in action.parent.GetComponentsInChildren<Card>())
		{
			if (card.cardVisual != null)
				card.cardVisual.UpdateIndex(action.parent.childCount);
		}

		yield return null;
	}

	private IEnumerator DestroyCardPerformer(DestroyCardGA action)
	{
		if (action.card != null)
		{
			HorizontalCardHolder holder = action.card.GetComponentInParent<HorizontalCardHolder>();
			if (holder != null)
				holder.cards.Remove(action.card);

			if (action.card.cardVisual != null)
			{
				DOTween.Kill(action.card.cardVisual.transform);
				Destroy(action.card.cardVisual.gameObject);
			}
			Destroy(action.card.transform.parent.gameObject);
		}
		yield return null;
	}

	private IEnumerator SpawnCardPerformer(SpawnCardGA action)
	{
		action.targetHolder.SpawnCards(action.amount);
		yield return null;
	}
}