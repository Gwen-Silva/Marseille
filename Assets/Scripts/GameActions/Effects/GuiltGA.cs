/// <summary>
/// Represents a GameAction that activates the Guilt effect from a card.
/// </summary>
public class GuiltGA : GameAction
{
	#region Public Fields
	public Card Card;
	#endregion

	#region Constructor
	public GuiltGA(Card card)
	{
		Card = card;
	}
	#endregion
}
