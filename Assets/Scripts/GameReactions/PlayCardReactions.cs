using System.Linq;
using UnityEngine;

public class PlayCardReactions : MonoBehaviour
{
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;
	[SerializeField] private TurnSystem turnSystem;

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<PlayCardGA>(PlayCardReactionPOST, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<PlayCardGA>(PlayCardReactionPOST, ReactionTiming.POST);
	}

	private void PlayCardReactionPOST(PlayCardGA action)
	{
		if (action.Card == null)
			return;

		CardDisplay cardDisplay = action.Card;
		Card card = cardDisplay.OwnerCard;
		bool isPlayerCard = card.isPlayerCard;

		if (action.IsValueSlot)
		{
			int value = int.Parse(cardDisplay.cardTopValue.text);
		}

		ActionSystem.Instance.AddReaction(new AdvanceTurnGA());
	}
}
