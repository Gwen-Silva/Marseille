/// <summary>
/// Represents a GameAction that activates the Love effect from a card.
/// </summary>
public class LoveGA : GameAction
{
	#region Public Fields
	public Card Card;
	#endregion

	#region Constructor
	public LoveGA(Card card)
	{
		Card = card;
	}
	#endregion
}
