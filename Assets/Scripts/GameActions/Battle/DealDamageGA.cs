/// <summary>
/// Represents a GameAction that deals damage from an attacker to a defender, affecting the target's health display.
/// </summary>
public class DealDamageGA : GameAction
{
	#region Public Fields
	public CardDisplay attackerCard;
	public CardDisplay defenderCard;
	public HealthDisplay target;
	public int amount;
	#endregion

	#region Constructor
	public DealDamageGA(CardDisplay attackerCard, CardDisplay defenderCard, HealthDisplay target, int amount)
	{
		this.attackerCard = attackerCard;
		this.defenderCard = defenderCard;
		this.target = target;
		this.amount = amount;
	}
	#endregion
}