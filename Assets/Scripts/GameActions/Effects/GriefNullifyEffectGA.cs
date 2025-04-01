/// <summary>
/// Represents a GameAction that nullifies effects based on Grief card tier.
/// </summary>
public class GriefNullifyEffectGA : GameAction
{
	#region Public Fields
	public int Tier;
	public bool TargetIsPlayer;
	#endregion

	#region Constructor
	public GriefNullifyEffectGA(int tier, bool targetIsPlayer)
	{
		Tier = tier;
		TargetIsPlayer = targetIsPlayer;
	}
	#endregion
}
