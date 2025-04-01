/// <summary>
/// Represents a GameAction that destroys a card.
/// </summary>
public class DestroyCardGA : GameAction
{
	#region Public Fields
	public Card card;
	#endregion

	#region Constructor
	public DestroyCardGA(Card card)
	{
		this.card = card;
	}
	#endregion
}