public class DealDamageGA : GameAction
{
	public CardDisplay AttackerCard;
	public CardDisplay DefenderCard;
	public HealthDisplay Target;
	public int Amount;

	public DealDamageGA(CardDisplay attackerCard, CardDisplay defenderCard, HealthDisplay target, int amount)
	{
		AttackerCard = attackerCard;
		DefenderCard = defenderCard;
		Target = target;
		Amount = amount;
	}
}
