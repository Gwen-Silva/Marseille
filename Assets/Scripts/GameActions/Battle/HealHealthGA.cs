/// <summary>
/// Represents a GameAction where health is restored to a target.
/// </summary>
public class HealHealthGA : GameAction
{
	#region Public Fields
	public HealthDisplay Target;
	public int Amount;
	#endregion

	#region Constructor
	public HealHealthGA(HealthDisplay target, int amount)
	{
		Target = target;
		Amount = amount;
	}
	#endregion
}