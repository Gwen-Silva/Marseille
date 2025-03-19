using UnityEngine;
using System.Linq;

public class OpponentAI : MonoBehaviour {
	public OpponentHandManager handManager;
	public TurnManager turnManager;

	private void Update() {
		if (turnManager.IsOpponentTurn()) {
			Debug.Log("Turno do Oponente - Preparando jogada.");
			PlayCard();
		}
	}

	private void PlayCard() {
		if (handManager.cardsInHand.Count > 0) {
			CardDisplay cardToPlay = ChooseBestCard();

			Transform targetSlot = turnManager.GetExpectedDropZone();
			if (targetSlot != null) {
				cardToPlay.transform.position = targetSlot.position;
				cardToPlay.GetComponent<CardMovement>().LockCardInPlace();

				CardDropZone dropZone = targetSlot.GetComponent<CardDropZone>();
				if (dropZone != null) {
					dropZone.OnCardDropped(cardToPlay);
				}

				Debug.Log("Oponente jogou a carta: " + cardToPlay.cardData.cardValue + " no slot " + targetSlot.name);

				handManager.RemoveCardFromHand(cardToPlay);
				turnManager.AdvanceTurn();
			}
		}
	}

	private CardDisplay ChooseBestCard() {
		return handManager.cardsInHand.OrderByDescending(card => card.cardData.cardValue).FirstOrDefault();
	}
}
