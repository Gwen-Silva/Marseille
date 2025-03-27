using UnityEngine;

public class SwapCardGA : GameAction
{
	public Card sourceCard;
	public Card targetCard;
	public Transform parent;

	public SwapCardGA(Card sourceCard, Card targetCard, Transform parent)
	{
		this.sourceCard = sourceCard;
		this.targetCard = targetCard;
		this.parent = parent;
	}
}