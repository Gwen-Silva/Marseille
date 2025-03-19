using AxolotlProductions;
using UnityEngine;

public class CardDropZone : MonoBehaviour, ICardDropArea {
	private bool cardPlaced = false;

	public bool isValueSlot;

	public void OnCardDropped(CardDisplay cardDisplay) {
		if (cardPlaced) return;

		Transform expectedDropZone = TurnManager.Instance.GetExpectedDropZone();

		if (expectedDropZone == transform) {
			cardDisplay.transform.position = transform.position;
			cardDisplay.GetComponent<CardMovement>().SetScaleForBoard();

			if (isValueSlot) {
				cardDisplay.ChangeToValueSprite();
			}

			TurnManager.Instance.AdvanceTurn();
			cardPlaced = true;
		}
		else {
			Debug.Log("Carta jogada no lugar errado! Retornando para a posição original.");
			cardDisplay.GetComponent<CardMovement>().TransitionToState0();
		}
	}
}
