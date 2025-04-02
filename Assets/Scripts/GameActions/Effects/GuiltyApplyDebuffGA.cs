/// <summary>
/// Represents a GameAction that reduces the opponent's argument value based on Guilt tier.
/// </summary>
public class GuiltApplyDebuffGA : GameAction
{
	#region Public Fields
	public bool TargetIsPlayer;
	public int Tier;
	#endregion

	#region Constructor
	public GuiltApplyDebuffGA(int tier, bool targetIsPlayer)
	{
		Tier = tier;
		TargetIsPlayer = targetIsPlayer;
	}
	#endregion
}
