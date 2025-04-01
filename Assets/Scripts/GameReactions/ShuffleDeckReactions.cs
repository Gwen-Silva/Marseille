using UnityEngine;

public class ShuffleDeckReactions : MonoBehaviour
{
	#region Constants

	private const int INITIAL_HAND_SIZE = 6;

	#endregion

	#region Cached References

	private DeckSystem deckSystem;
	private GameSystem gameSystem;

	#endregion

	#region Unity Events

	/// <summary>
	/// Subscribes to ShuffleDeckGA reactions.
	/// </summary>
	private void OnEnable()
	{
		deckSystem = FindFirstObjectByType<DeckSystem>();
		gameSystem = FindFirstObjectByType<GameSystem>();

		ActionSystem.SubscribeReaction<ShuffleDeckGA>(ShuffleDeckReaction, ReactionTiming.POST);
	}

	/// <summary>
	/// Unsubscribes from ShuffleDeckGA reactions.
	/// </summary>
	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ShuffleDeckGA>(ShuffleDeckReaction, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Executes the reaction to shuffle a deck. Triggers drawing initial cards if it's the opponent's deck.
	/// </summary>
	private void ShuffleDeckReaction(ShuffleDeckGA ga)
	{
		if (ga.deck == deckSystem.opponentDeck)
		{
			var drawInitial = new DrawInitialCardsGA(gameSystem.PlayerHand, gameSystem.OpponentHand, INITIAL_HAND_SIZE);
			ActionSystem.Instance.AddReaction(drawInitial);
		}
	}

	#endregion
}
