using System.Linq;
using UnityEngine;

public class ClearBoardReactions : MonoBehaviour
{
	#region Constants

	private const float CARD_FLIP_DELAY = 0.1f;
	private const string BOARD_SLOT_TAG = "BoardSlot";

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<ClearBoardGA>(ClearBoardReaction, ReactionTiming.PRE);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ClearBoardGA>(ClearBoardReaction, ReactionTiming.PRE);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Reaction executed before the action of clearing the board; flips the cards present in the slots.
	/// </summary>
	/// <param name=\"ga\">The action of clearing the board.</param>
	private void ClearBoardReaction(ClearBoardGA ga)
	{
		var occupiedBoardSlots = GameObject
			.FindObjectsByType<CardDropZone>(FindObjectsSortMode.None)
			.Where(slot => slot.CompareTag(BOARD_SLOT_TAG) && slot.transform.childCount > 0)
			.ToList();

		foreach (var slot in occupiedBoardSlots)
		{
			Card card = slot.GetComponentInChildren<Card>();
			if (card != null && card.cardVisual != null)
			{
				ActionSystem.Instance.AddReaction(new FlipCardGA(card, CARD_FLIP_DELAY));
			}
		}
	}

	#endregion
}
