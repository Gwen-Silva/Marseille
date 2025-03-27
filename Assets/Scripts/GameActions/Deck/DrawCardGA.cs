using System.Collections.Generic;

public class DrawCardGA : GameAction
{
	public HorizontalCardHolder targetHolder;
	public int amount;
	public List<Card> spawnedCards = new();

	public DrawCardGA(HorizontalCardHolder targetHolder, int amount)
	{
		this.targetHolder = targetHolder;
		this.amount = amount;
	}
}
