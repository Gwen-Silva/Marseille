using UnityEngine;

public class DrawCardButton : MonoBehaviour
{
	#region Constants

	private const int DEFAULT_CARDS_TO_SPAWN = 1;

	#endregion

	#region Serialized Fields

	public HorizontalCardHolder targetHolder;
	[SerializeField] private ActionSystem actionSystem;

	#endregion

	#region Private Fields

	private int cardsToSpawn = DEFAULT_CARDS_TO_SPAWN;

	#endregion

	#region Public Methods

	/// <summary>
	/// Draws a card from the deck into the specified target holder.
	/// </summary>
	public void Draw()
	{
		if (actionSystem.IsPerforming) return;

		DrawCardGA drawCardGA = new(targetHolder, cardsToSpawn);
		actionSystem.Perform(drawCardGA);
	}

	#endregion
}
