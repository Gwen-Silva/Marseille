using UnityEngine;

public class ShuffleDecksOnGenerate : MonoBehaviour
{
	private DeckSystem deckSystem;

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<GenerateDecksGA>(GenerateDecksReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<GenerateDecksGA>(GenerateDecksReaction, ReactionTiming.POST);
	}

	private void GenerateDecksReaction(GenerateDecksGA action)
	{
		Debug.Log("Calling Shuffle Deck Reaction");
		ActionSystem.Instance.AddReaction(new ShuffleDeckGA(deckSystem.playerDeck));
		ActionSystem.Instance.AddReaction(new ShuffleDeckGA(deckSystem.opponentDeck));
	}
}
