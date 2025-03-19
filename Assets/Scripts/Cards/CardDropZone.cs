using AxolotlProductions;
using UnityEngine;
using UnityEngine.UI;

public class CardDropZone : MonoBehaviour, ICardDropArea {
	private bool cardPlaced = false;
	public bool isValueSlot;

	[SerializeField] private GameObject gloweffect;
	[SerializeField] private Image slotImage;
	[SerializeField] private Sprite valueSlotSprite;
	[SerializeField] private Sprite effectSlotSprite;

	private void Start() {
		ToggleHighlight(false);
		UpdateSlotSprite();
	}

	private void UpdateSlotSprite() {
		if (slotImage != null) {
			slotImage.sprite = isValueSlot ? valueSlotSprite : effectSlotSprite;
		}
	}

	public void ToggleHighlight(bool state) {
		if (gloweffect != null) {
			gloweffect.SetActive(state);
		}
	}

	public void OnCardDropped(CardDisplay cardDisplay) {
		if (cardPlaced) return;

		Transform expectedDropZone = TurnManager.Instance.GetExpectedDropZone();

		if (expectedDropZone == transform) {
			cardDisplay.transform.position = transform.position;
			cardDisplay.transform.rotation = transform.rotation;
			cardDisplay.GetComponent<CardMovement>().SetScaleForBoard();

			if (isValueSlot) {
				cardDisplay.UpdateCardDisplay();
				cardDisplay.ChangeToValueSprite();
				var topValueText = cardDisplay.cardTopValue.GetComponent<RectTransform>();
				topValueText.anchoredPosition = Vector2.zero;
				cardDisplay.cardTopValue.fontSize = 1;
				cardDisplay.cardBottomValue.gameObject.SetActive(false);
			}

			HandManager handManager = FindFirstObjectByType<HandManager>();
			OpponentHandManager opponentHandManager = FindFirstObjectByType<OpponentHandManager>();

			if (handManager != null && handManager.cardsInHand.Contains(cardDisplay)) {
				handManager.RemoveCardFromHand(cardDisplay);
			}
			else if (opponentHandManager != null && opponentHandManager.cardsInHand.Contains(cardDisplay)) {
				opponentHandManager.RemoveCardFromHand(cardDisplay);
			}

			cardDisplay.GetComponent<CardMovement>().LockCardInPlace();

			TurnManager.Instance.AdvanceTurn();
			TurnManager.Instance.UpdateSlotHighlight();

			cardPlaced = true;
		}
		else {
			cardDisplay.GetComponent<CardMovement>().TransitionToState0();
		}
	}
}
