/// <summary>
/// Represents a GameAction that deals damage from an attacker to a defender, affecting the target's health display.
/// </summary>
public class DealDamageGA : GameAction
{
	#region Public Fields
	public CardDisplay AttackerCard;
	public CardDisplay DefenderCard;
	public HealthDisplay Target;
	public int Amount;
	#endregion

	#region Constructor
	public DealDamageGA(CardDisplay attackerCard, CardDisplay defenderCard, HealthDisplay target, int amount)
	{
		AttackerCard = attackerCard;
		DefenderCard = defenderCard;
		Target = target;
		Amount = amount;
	}
	#endregion
}