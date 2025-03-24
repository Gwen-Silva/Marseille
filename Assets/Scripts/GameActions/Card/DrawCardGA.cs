using UnityEngine;

public class DrawCardGA : GameAction
{
	public int Amount;
	public HorizontalCardHolder targetHolder;

	public DrawCardGA(int amount, HorizontalCardHolder targetHolder)
	{
		Amount = amount;
		this.targetHolder = targetHolder;
	}
}
