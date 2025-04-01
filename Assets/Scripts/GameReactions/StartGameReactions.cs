using UnityEngine;

public class StartGameReactions : MonoBehaviour
{
	#region Constants

	private const int DEFAULT_CARDS_TO_DRAW = 2;

	#endregion

	#region Serialized Fields

	[SerializeField] private int cardsToDraw = DEFAULT_CARDS_TO_DRAW;

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
		var cardSystem = FindFirstObjectByType<CardSystem>();
		var turnSystem = FindFirstObjectByType<TurnSystem>();

		if (turnSystem != null && turnSystem.IsFirstRound)
		{
			return;
		}

		ActionSystem.Instance.AddReaction(new DrawCardGA(cardSystem.PlayerCardHolder, cardsToDraw));
		ActionSystem.Instance.AddReaction(new DrawCardGA(cardSystem.OpponentCardHolder, cardsToDraw));
	}

	#endregion
}
