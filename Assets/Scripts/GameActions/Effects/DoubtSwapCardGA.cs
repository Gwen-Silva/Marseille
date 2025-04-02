using System.Collections.Generic;

public class DoubtSwapCardsGA : GameAction
{
	public int Amount;
	public bool IsPlayer;
	public bool IsUltimate;
	public List<Card> cardsToSwap;

	public DoubtSwapCardsGA(int amount, bool isPlayer, bool isUltimate = false)
	{
		Amount = amount;
		IsPlayer = isPlayer;
		IsUltimate = isUltimate;
	}
}
