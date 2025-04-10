using UnityEngine;

public class GriefReactions : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private ActionSystem actionSystem;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<GriefApplyShieldGA>(GriefShieldReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<GriefApplyShieldGA>(GriefShieldReaction, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Reaction executed after applying a grief shield. (Implementation pending)
	/// </summary>
	/// <param name="ga">The action of applying a grief shield.</param>
	private void GriefShieldReaction(GriefApplyShieldGA ga)
	{
		
	}

	#endregion
}
