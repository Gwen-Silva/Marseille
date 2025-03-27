using System.Collections.Generic;

public class ShuffleDeckGA : GameAction
{
	public List<CardData> deck;

	public ShuffleDeckGA(List<CardData> deck)
	{
		this.deck = deck;
	}
}
