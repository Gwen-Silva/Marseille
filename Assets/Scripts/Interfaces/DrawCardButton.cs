using UnityEngine;

public class DrawCardButton : MonoBehaviour
{
	public HorizontalCardHolder targetHolder;

	public void Draw()
	{
		if (ActionSystem.Instance.IsPerforming) return;

		DrawCardGA drawCardGA = new(1, targetHolder);
		ActionSystem.Instance.Perform(drawCardGA);
	}
}
