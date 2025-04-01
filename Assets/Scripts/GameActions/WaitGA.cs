/// <summary>
/// Represents a GameAction that introduces a delay in the execution flow, based on a predefined delay level.
/// </summary>
public class WaitGA : GameAction
{
	#region Public Fields
	public DelayType DelayLevel;
	#endregion

	#region Constructor
	public WaitGA(DelayType delay)
	{
		DelayLevel = delay;
	}
	#endregion
}
