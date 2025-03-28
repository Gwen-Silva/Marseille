using UnityEngine;

public class ShuffleDeckReactionsPOST : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<ShuffleDeckGA>(ShuffleDeckReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ShuffleDeckGA>(ShuffleDeckReaction, ReactionTiming.POST);
	}

	private void ShuffleDeckReaction(ShuffleDeckGA ga)
	{
		var deckSystem = FindFirstObjectByType<DeckSystem>();
		var gameSystem = FindFirstObjectByType<GameSystem>();

		if (ga.deck == deckSystem.opponentDeck)
		{
			var drawInitial = new DrawInitialCardsGA(gameSystem.PlayerHand, gameSystem.OpponentHand, 6);
			ActionSystem.Instance.AddReaction(drawInitial);
		}
	}
}
