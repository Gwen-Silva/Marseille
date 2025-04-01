/// <summary>
/// Represents a GameAction that draws an initial set of cards for both the player and the opponent.
/// </summary>
public class DrawInitialCardsGA : GameAction
{
	#region Public Fields
	public HorizontalCardHolder playerHand;
	public HorizontalCardHolder opponentHand;
	public int amount;
	#endregion

	#region Constructor
	public DrawInitialCardsGA(HorizontalCardHolder playerHand, HorizontalCardHolder opponentHand, int amount)
	{
		this.playerHand = playerHand;
		this.opponentHand = opponentHand;
		this.amount = amount;
	}
	#endregion
}
