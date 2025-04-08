using UnityEngine;
using UnityEngine.UI;

public class CardDropZone : MonoBehaviour, ICardDropArea
{
	#region Serialized Fields

	[Header("Slot Configuration")]
	public bool IsPlayerSlot = true;
	public bool isValueSlot;

	[Header("Visual References")]
	[SerializeField] private GameObject glowEffect;
	[SerializeField] private Image slotImage;
	[SerializeField] private Sprite valueSlotSprite;
	[SerializeField] private Sprite effectSlotSprite;

	[Header("Dependencies")]
	[SerializeField] private TurnSystem turnSystem;

	#endregion

	#region Private Fields

	private bool cardPlaced = false;

	#endregion

	#region Unity Methods

	private void Start()
	{
		ToggleHighlight(false);
		UpdateSlotSprite();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Enables or disables the visual highlight around the slot.
	/// </summary>
	public void ToggleHighlight(bool state)
	{
		if (glowEffect != null)
			glowEffect.SetActive(state);
	}

	/// <summary>
	/// Called when a card is dropped onto this slot.
	/// </summary>
	public void OnCardDropped(Card card)
	{
		if (cardPlaced || card == null || turnSystem.CurrentActiveSlot != this)
			return;

		if (card.isPlayerCard != IsPlayerSlot)
			return;

		var cardDisplay = card.cardVisual.GetComponent<CardDisplay>();

		ActionSystem.Shared?.Perform(new PlayCardGA
		{
			Card = cardDisplay,
			TargetSlot = this,
			IsValueSlot = isValueSlot
		});

		cardPlaced = true;
		ToggleHighlight(false);
	}

	/// <summary>
	/// Resets the slot to be ready for a new card.
	/// </summary>
	public void ResetSlot()
	{
		cardPlaced = false;
	}

	#endregion

	#region Private Methods

	private void UpdateSlotSprite()
	{
		if (slotImage != null)
			slotImage.sprite = isValueSlot ? valueSlotSprite : effectSlotSprite;
	}

	#endregion
}
