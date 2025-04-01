/// <summary>
/// Represents a GameAction that applies a shield using a Grief effect card.
/// </summary>
public class GriefApplyShieldGA : GameAction
{
	#region Public Fields
	public Card Card;
	#endregion

	#region Constructor
	public GriefApplyShieldGA(Card card)
	{
		Card = card;
	}
	#endregion
}
