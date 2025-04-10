using UnityEngine;

public class ShuffleDeckReactions : MonoBehaviour
{
	#region Constants

	private const int INITIAL_HAND_SIZE = 6;

	#endregion

	#region Serialized Fields

	[SerializeField] private ActionSystem actionSystem;
	[SerializeField] private DeckSystem deckSystem;
	[SerializeField] private GameSystem gameSystem;

	#endregion

	#region Unity Events

	/// <summary>
	/// Subscribes to ShuffleDeckGA reactions.
	/// </summary>
	private void OnEnable()
	{
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
			actionSystem.AddReaction(drawInitial);
		}
	}

	#endregion
}
