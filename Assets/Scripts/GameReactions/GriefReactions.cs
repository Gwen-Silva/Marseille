using UnityEngine;

public class GriefReactions : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<GriefApplyShieldGA>(GriefShieldReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<GriefApplyShieldGA>(GriefShieldReaction, ReactionTiming.POST);
	}

	private void GriefShieldReaction(GriefApplyShieldGA ga)
	{

	}
}
