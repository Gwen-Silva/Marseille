using UnityEngine;
using System.Collections.Generic;
using AxolotlProductions;

public abstract class BaseHandManager : MonoBehaviour {
	public GameObject cardPrefab;
	public Transform handTransform;
	public float fanSpread = 7.5f;
	public float horizontalSpacing = 100f;
	public float verticalSpacing = 100f;

	public List<CardDisplay> cardsInHand = new List<CardDisplay>();

	public virtual void AddCardToHand(Card cardData) {
		GameObject newCardObj = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
		CardDisplay newCard = newCardObj.GetComponent<CardDisplay>();

		if (newCard != null) {
			newCard.cardData = cardData;
			newCard.UpdateCardDisplay();
			cardsInHand.Add(newCard);
		}

		UpdateHandVisuals();
	}

	public virtual void RemoveCardFromHand(CardDisplay card) {
		if (cardsInHand.Contains(card)) {
			cardsInHand.Remove(card);
			UpdateHandVisuals();
		}
	}

	protected virtual void UpdateHandVisuals() {
		int cardCount = cardsInHand.Count;

		if (cardCount == 1) {
			cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			cardsInHand[0].transform.localPosition = Vector3.zero;
			return;
		}

		for (int i = 0; i < cardCount; i++) {
			float rotationAngle = fanSpread * (i - (cardCount - 1) / 2f);
			cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

			float horizontalOffset = horizontalSpacing * (i - (cardCount - 1) / 2f);
			float normalizedPosition = (2f * i / (cardCount - 1) - 1f);
			float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

			cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
		}
	}
}
