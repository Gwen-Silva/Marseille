public enum CombatResult
{
	AttackSuccess,
	AttackBlocked,
	Tie
}

public class CardCombatAnimationGA : GameAction
{
	public Card attacker;
	public Card defender;
	public CombatResult result;

	public CardCombatAnimationGA(Card attacker, Card defender, CombatResult result)
	{
		this.attacker = attacker;
		this.defender = defender;
		this.result = result;
	}
}
