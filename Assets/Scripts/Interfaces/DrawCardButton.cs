using UnityEngine;

public class DrawCardButton : MonoBehaviour
{
	public HorizontalCardHolder targetHolder;
	private int cardsToSpawn = 1;

	public void Draw()
	{
		if (ActionSystem.Instance.IsPerforming) return;

		DrawCardGA drawCardGA = new(targetHolder, cardsToSpawn);
		ActionSystem.Instance.Perform(drawCardGA);
	}
}
