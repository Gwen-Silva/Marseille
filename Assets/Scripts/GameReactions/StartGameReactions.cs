using UnityEngine;

public class StartGameReactions : MonoBehaviour
{
	[SerializeField] private int cardsToDraw = 2;

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<StartGameGA>(StartGameReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<StartGameGA>(StartGameReaction, ReactionTiming.POST);
	}

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

		Debug.Log($"[StartGameDrawReaction] {cardsToDraw} cartas compradas para cada jogador.");
	}

}