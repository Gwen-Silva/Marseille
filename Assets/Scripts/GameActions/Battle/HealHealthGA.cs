using UnityEngine;

public class HealHealthGA : GameAction
{
	public HealthDisplay Target;
	public int Amount;

	public HealHealthGA(HealthDisplay target, int amount)
	{
		Target = target;
		Amount = amount;
	}
}
