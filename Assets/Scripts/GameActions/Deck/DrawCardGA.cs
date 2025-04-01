using System.Collections.Generic;

/// <summary>
/// Represents a GameAction that draws a specific number of cards to a target hand.
/// </summary>
public class DrawCardGA : GameAction
{
	#region Public Fields
	public HorizontalCardHolder targetHolder;
	public int amount;
	public List<Card> spawnedCards = new();
	#endregion

	#region Constructor
	public DrawCardGA(HorizontalCardHolder targetHolder, int amount)
	{
		this.targetHolder = targetHolder;
		this.amount = amount;
	}
	#endregion
}