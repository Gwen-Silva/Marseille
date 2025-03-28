public class FlipCardGA : GameAction
{
	public Card card;
	public float duration = 0.25f;

	public FlipCardGA(Card card, float duration)
	{
		this.card = card;
		this.duration = duration;
	}
}
