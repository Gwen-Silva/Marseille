public class SpawnCardGA : GameAction
{
	public HorizontalCardHolder targetHolder;
	public int amount;

	public SpawnCardGA(HorizontalCardHolder targetHolder, int amount = 1)
	{
		this.targetHolder = targetHolder;
		this.amount = amount;
	}
}
