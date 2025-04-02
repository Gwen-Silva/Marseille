using System.Collections.Generic;

/// <summary>
/// Represents a GameAction that draws a specific number of cards to a target hand.
/// </summary>
public class DrawCardGA : GameAction
{
	#region Public Fields
	public HorizontalCardHolder targetHolder;
	public int amount;

	/// <summary>
	/// If set, forces the draw of a card with this specific value.
	/// </summary>
	public int? forcedValue;

	public List<Card> spawnedCards = new();
	#endregion

	#region Constructor
	public DrawCardGA(HorizontalCardHolder targetHolder, int amount)
	{
		this.targetHolder = targetHolder;
		this.amount = amount;
		this.forcedValue = null;
	}

	public DrawCardGA(HorizontalCardHolder targetHolder, int amount, int forcedValue)
	{
		this.targetHolder = targetHolder;
		this.amount = amount;
		this.forcedValue = forcedValue;
	}
	#endregion
}