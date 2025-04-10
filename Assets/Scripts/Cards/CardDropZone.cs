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

	#endregion

	#region Private Fields

	private bool cardPlaced = false;

	private ActionSystem actionSystem;
	private TurnSystem turnSystem;
	private DeckSystem deckSystem;

	#endregion

	#region Unity Methods

	private void Start()
	{
		// 🔄 Injeção automática das dependências
		actionSystem = Object.FindFirstObjectByType<ActionSystem>();
		turnSystem = Object.FindFirstObjectByType<TurnSystem>();
		deckSystem = Object.FindFirstObjectByType<DeckSystem>();

		ToggleHighlight(false);
		UpdateSlotSprite();
	}

	#endregion

	#region Public Methods

	public void ToggleHighlight(bool state)
	{
		if (glowEffect != null)
			glowEffect.SetActive(state);
	}

	public void OnCardDropped(Card card)
	{
		if (cardPlaced || card == null || turnSystem.CurrentActiveSlot != this)
			return;

		if (card.isPlayerCard != IsPlayerSlot)
			return;

		var cardDisplay = card.cardVisual.GetComponent<CardDisplay>();

		actionSystem.Perform(new PlayCardGA
		{
			Card = cardDisplay,
			TargetSlot = this,
			IsValueSlot = isValueSlot
		});

		cardPlaced = true;
		ToggleHighlight(false);
	}

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
