using DG.Tweening;
using System.Linq;
using UnityEngine;

public class ClearBoardReactionsPRE : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<ClearBoardGA>(ClearBoardReaction, ReactionTiming.PRE);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ClearBoardGA>(ClearBoardReaction, ReactionTiming.PRE);
	}

	private void ClearBoardReaction(ClearBoardGA ga)
	{
		var boardSlots = GameObject.FindObjectsByType<CardDropZone>(FindObjectsSortMode.None)
			.Where(slot => slot.CompareTag("BoardSlot"))
			.ToList();

		foreach (var slot in boardSlots)
		{
			if (slot.transform.childCount > 0)
			{
				Card card = slot.GetComponentInChildren<Card>();
				if (card != null && card.cardVisual != null)
				{
					ActionSystem.Instance.AddReaction(new FlipCardGA(card, 0.12f));
				}
			}
		}
	}
}
