using UnityEngine;
using UnityEngine.UI;

public class CardDropZone : MonoBehaviour, ICardDropArea
{
	public bool IsPlayerSlot = true;
	private bool cardPlaced = false;
	public bool isValueSlot;

	[Header("References")]
	[SerializeField] private GameObject gloweffect;
	[SerializeField] private Image slotImage;
	[SerializeField] private Sprite valueSlotSprite;
	[SerializeField] private Sprite effectSlotSprite;

	[Header("Dependencies")]
	[SerializeField] private TurnSystem turnSystem;


	private void Start()
	{
		ToggleHighlight(false);
		UpdateSlotSprite();
	}

	private void UpdateSlotSprite()
	{
		if (slotImage != null)
		{
			slotImage.sprite = isValueSlot ? valueSlotSprite : effectSlotSprite;
		}
	}

	public void ToggleHighlight(bool state)
	{
		if (gloweffect != null)
		{
			gloweffect.SetActive(state);
		}
	}

	public void OnCardDropped(Card card)
	{
		if (cardPlaced || card == null)
			return;

		if (turnSystem.CurrentActiveSlot != this)
		{
			Debug.Log($"[🚫] Tentativa de jogar em slot errado: {name}");
			return;
		}

		bool isCardFromPlayer = card.isPlayerCard;

		if (isCardFromPlayer != IsPlayerSlot)
			return;

		var cardDisplay = card.cardVisual.GetComponent<CardDisplay>();

		ActionSystem.Instance.Perform(new PlayCardGA
		{
			Card = cardDisplay,
			TargetSlot = this,
			IsValueSlot = isValueSlot
		});

		cardPlaced = true;
		ToggleHighlight(false);
	}
}
