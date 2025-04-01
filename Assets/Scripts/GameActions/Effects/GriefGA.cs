/// <summary>
/// Represents a GameAction that activates the Grief effect from a card.
/// </summary>
public class GriefGA : GameAction
{
	#region Public Fields
	public Card Card;
	#endregion

	#region Constructor
	public GriefGA(Card card)
	{
		Card = card;
	}
	#endregion
}
