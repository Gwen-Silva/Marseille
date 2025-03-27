public class DrawInitialCardsGA : GameAction
{
	public HorizontalCardHolder playerHand;
	public HorizontalCardHolder opponentHand;
	public int amount;

	public DrawInitialCardsGA(HorizontalCardHolder playerHand, HorizontalCardHolder opponentHand, int amount)
	{
		this.playerHand = playerHand;
		this.opponentHand = opponentHand;
		this.amount = amount;
	}
}
