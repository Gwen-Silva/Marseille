public class OpponentHand : HorizontalCardHolder
{
	protected override void Start()
	{
		base.Start();
		DisableCardInteractions();
	}

	private void DisableCardInteractions()
	{
		foreach (var card in cards)
		{
			card.DisableInteraction();
		}
	}
}
