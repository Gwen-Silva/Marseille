/// <summary>
/// Represents a GameAction for animating card combat between an attacker and a defender.
/// </summary>
public class CardCombatAnimationGA : GameAction
{
	#region Public Fields
	public Card attacker;
	public Card defender;
	public CombatResult result;
	#endregion

	#region Constructor
	public CardCombatAnimationGA(Card attacker, Card defender, CombatResult result)
	{
		this.attacker = attacker;
		this.defender = defender;
		this.result = result;
	}
	#endregion
}

/// <summary>
/// Enum to represent possible combat outcomes between two cards.
/// </summary>
public enum CombatResult
{
	AttackSuccess,
	AttackBlocked,
	Tie
}
