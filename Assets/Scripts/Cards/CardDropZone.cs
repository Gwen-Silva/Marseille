using UnityEngine;
using UnityEngine.UI;

public class CardDropZone : MonoBehaviour, ICardDropArea
{
	private bool cardPlaced = false;
	public bool isValueSlot;

	[SerializeField] private GameObject gloweffect;
	[SerializeField] private Image slotImage;
	[SerializeField] private Sprite valueSlotSprite;
	[SerializeField] private Sprite effectSlotSprite;

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

	public void OnCardDropped(CardDisplay cardDisplay)
	{
		if (cardPlaced || cardDisplay == null)
			return;

		ActionSystem.Instance.Perform(new PlayCardGA
		{
			Card = cardDisplay,
			TargetSlot = this,
			IsValueSlot = isValueSlot
		});

		cardPlaced = true;
		ToggleHighlight(false);
	}

	public void ForceResetSlot()
	{
		cardPlaced = false;
		ToggleHighlight(false);
	}
}
