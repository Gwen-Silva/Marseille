using UnityEngine;

public class GriefNullifyEffectGA : GameAction
{
	public int Tier;
	public bool TargetIsPlayer;

	public GriefNullifyEffectGA(int tier, bool targetIsPlayer)
	{
		Tier = tier;
		TargetIsPlayer = targetIsPlayer;
	}
}
