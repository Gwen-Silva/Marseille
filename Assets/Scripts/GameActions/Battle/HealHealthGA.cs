/// <summary>
/// Represents a GameAction where health is restored to a target.
/// </summary>
public class HealHealthGA : GameAction
{
	#region Public Fields
	public HealthDisplay target;
	public int amount;
	#endregion

	#region Constructor
	public HealHealthGA(HealthDisplay target, int amount)
	{
		this.target = target;
		this.amount = amount;
	}
	#endregion
}