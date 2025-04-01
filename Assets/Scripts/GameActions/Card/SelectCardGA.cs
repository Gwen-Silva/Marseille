/// <summary>
/// Represents a GameAction that selects a card.
/// </summary>
public class SelectCardGA : GameAction
{
	#region Public Fields
	public Card card;
	#endregion

	#region Constructor
	public SelectCardGA(Card card)
	{
		this.card = card;
	}
	#endregion
}