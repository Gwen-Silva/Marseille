/// <summary>
/// Represents a GameAction that flips a card with an optional animation duration.
/// </summary>
public class FlipCardGA : GameAction
{
	#region Public Fields
	public Card card;
	public float duration = 0.25f;
	#endregion

	#region Constructor
	public FlipCardGA(Card card, float duration)
	{
		this.card = card;
		this.duration = duration;
	}
	#endregion
}