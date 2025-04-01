using System.Collections.Generic;

/// <summary>
/// Represents a GameAction that shuffles a given deck of card data.
/// </summary>
public class ShuffleDeckGA : GameAction
{
	#region Public Fields
	public List<CardData> deck;
	#endregion

	#region Constructor
	public ShuffleDeckGA(List<CardData> deck)
	{
		this.deck = deck;
	}
	#endregion
}
