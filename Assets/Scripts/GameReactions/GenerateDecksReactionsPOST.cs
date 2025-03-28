using UnityEngine;

public class GenerateDecksReactionsPOST : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<GenerateDecksGA>(GenerateDecksReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<GenerateDecksGA>(GenerateDecksReaction, ReactionTiming.POST);
	}

	private void GenerateDecksReaction(GenerateDecksGA ga)
	{
		var deckSystem = FindFirstObjectByType<DeckSystem>();
		var shufflePlayer = new ShuffleDeckGA(deckSystem.playerDeck);
		var shuffleOpponent = new ShuffleDeckGA(deckSystem.opponentDeck);

		ActionSystem.Instance.AddReaction(shufflePlayer);
		ActionSystem.Instance.AddReaction(shuffleOpponent);
	}
}
