using UnityEngine;
using System.Linq;

public class OpponentAI : MonoBehaviour
{
	public static OpponentAI Instance;
	public OpponentHandManager handManager;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void PlayCard()
	{
		if (handManager.cardsInHand.Count > 0)
		{
			CardDisplay cardToPlay = ChooseBestCard();
			Transform targetSlot = TurnManager.Instance.GetExpectedDropZone();

			if (targetSlot != null)
			{
				cardToPlay.transform.position = targetSlot.position;
				cardToPlay.GetComponent<CardMovement>().LockCardInPlace();

				CardDropZone dropZone = targetSlot.GetComponent<CardDropZone>();
				if (dropZone != null)
				{
					dropZone.OnCardDropped(cardToPlay);
				}

				Debug.Log("Oponente jogou a carta: " + cardToPlay.cardData.cardValue + " no slot " + targetSlot.name);

				handManager.RemoveCardFromHand(cardToPlay);
			}
		}
	}

	private CardDisplay ChooseBestCard()
	{
		return handManager.cardsInHand.OrderByDescending(card => card.cardData.cardValue).FirstOrDefault();
	}
}
