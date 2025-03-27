using UnityEngine;

public class PlayCardReactions : MonoBehaviour
{
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;
	[SerializeField] private TurnSystem turnSystem;

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<PlayCardGA>(PlayCardReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<PlayCardGA>(PlayCardReaction, ReactionTiming.POST);
	}

	private void PlayCardReaction(PlayCardGA action)
	{
		if (action.Card == null)
			return;

		CardDisplay cardDisplay = action.Card;
		bool isPlayerCard = cardDisplay.OwnerCard.isPlayerCard;

		if (action.IsValueSlot)
		{
			int value = int.Parse(cardDisplay.cardTopValue.text);
		}

		ActionSystem.Instance.AddReaction(new AdvanceTurnGA());
	}
}
