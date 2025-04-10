using UnityEngine;

public class GenerateDecksReactions : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private ActionSystem actionSystem;
	[SerializeField] private DeckSystem deckSystem;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<GenerateDecksGA>(GenerateDecksReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<GenerateDecksGA>(GenerateDecksReaction, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Reaction executed after generating decks; shuffles both player's and opponent's decks.
	/// </summary>
	/// <param name="ga">The action of generating decks.</param>
	private void GenerateDecksReaction(GenerateDecksGA ga)
	{
		var shufflePlayerDeck = new ShuffleDeckGA(deckSystem.playerDeck);
		var shuffleOpponentDeck = new ShuffleDeckGA(deckSystem.opponentDeck);

		actionSystem.AddReaction(shufflePlayerDeck);
		actionSystem.AddReaction(shuffleOpponentDeck);
	}

	#endregion
}