using UnityEngine;

public class StartGameReactions : MonoBehaviour
{
	#region Constants

	private const int DEFAULT_CARDS_TO_DRAW = 2;

	#endregion

	#region Serialized Fields

	[SerializeField] private int cardsToDraw = DEFAULT_CARDS_TO_DRAW;

	[SerializeField] private ActionSystem actionSystem;
	[SerializeField] private CardSystem cardSystem;
	[SerializeField] private TurnSystem turnSystem;

	#endregion

	#region Unity Events

	/// <summary>
	/// Subscribes to StartGameGA reactions.
	/// </summary>
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<StartGameGA>(StartGameReaction, ReactionTiming.POST);
	}

	/// <summary>
	/// Unsubscribes from StartGameGA reactions.
	/// </summary>
	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<StartGameGA>(StartGameReaction, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Executes the start game reaction. Draws cards for both players unless it's the first round.
	/// </summary>
	private void StartGameReaction(StartGameGA ga)
	{
		if (turnSystem != null && turnSystem.IsFirstRound)
		{
			return;
		}

		actionSystem.AddReaction(new DrawCardGA(cardSystem.PlayerCardHolder, cardsToDraw));
		actionSystem.AddReaction(new DrawCardGA(cardSystem.OpponentCardHolder, cardsToDraw));
	}

	#endregion
}
