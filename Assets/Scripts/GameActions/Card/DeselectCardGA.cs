/// <summary>
/// Represents a GameAction that deselects a card.
/// </summary>
public class DeselectCardGA : GameAction
{
	#region Public Fields
	public Card card;
	#endregion

	#region Constructor
	public DeselectCardGA(Card card)
	{
		this.card = card;
	}
	#endregion
}