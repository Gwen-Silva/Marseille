using UnityEngine;

public class PlayCardReactions : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;
	[SerializeField] private TurnSystem turnSystem;

	[SerializeField] private ActionSystem actionSystem;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<PlayCardGA>(PlayCardReactionPOST, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<PlayCardGA>(PlayCardReactionPOST, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Reaction executed after a card is played; handles the advancement of the turn.
	/// </summary>
	/// <param name="action">The action containing details of the played card.</param>
	private void PlayCardReactionPOST(PlayCardGA action)
	{
		if (action.Card == null)
			return;

		CardDisplay cardDisplay = action.Card;
		Card card = cardDisplay.OwnerCard;

		if (action.IsValueSlot)
		{
			int value = int.Parse(cardDisplay.cardTopValue.text);
		}

		actionSystem.AddReaction(new AdvanceTurnGA());
	}
	#endregion
}