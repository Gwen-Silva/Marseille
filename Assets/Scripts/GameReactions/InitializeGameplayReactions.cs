using UnityEngine;

public class InitializeGameplayReactions : MonoBehaviour
{
	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<InitializeGameplayGA>(InitializeGameplayReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<InitializeGameplayGA>(InitializeGameplayReaction, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Reaction executed after initializing the game.
	/// </summary>
	private void InitializeGameplayReaction(InitializeGameplayGA action)
	{
		ActionSystem.Instance.AddReaction(new GenerateDecksGA());
	}
	#endregion
}