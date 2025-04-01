using UnityEngine;

/// <summary>
/// Represents a GameAction that swaps two cards' positions in a given parent transform.
/// </summary>
public class SwapCardGA : GameAction
{
	#region Public Fields
	public Card sourceCard;
	public Card targetCard;
	public Transform parent;
	#endregion

	#region Constructor
	public SwapCardGA(Card sourceCard, Card targetCard, Transform parent)
	{
		this.sourceCard = sourceCard;
		this.targetCard = targetCard;
		this.parent = parent;
	}
	#endregion
}