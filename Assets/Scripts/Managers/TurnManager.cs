using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public static TurnManager Instance;

	public Transform[] playerDropZones;
	public Transform[] opponentDropZones;
	private int currentTurnIndex = 0;
	private bool isPlayerTurn = true;

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

	private void Start()
	{
		UpdateSlotHighlight();
	}

	public Transform GetExpectedDropZone()
	{
		Transform[] activeDropZones = isPlayerTurn ? playerDropZones : opponentDropZones;
		return currentTurnIndex < activeDropZones.Length ? activeDropZones[currentTurnIndex] : null;
	}

	public void OnCardPlayed()
	{
		AdvanceTurn();
	}

	public void AdvanceTurn()
	{
		if (isPlayerTurn)
		{
			isPlayerTurn = false;
			OpponentAI.Instance.PlayCard();
		}
		else
		{
			isPlayerTurn = true;
			currentTurnIndex++;

			if (currentTurnIndex >= playerDropZones.Length)
			{
				ResetTurn();
				return;
			}
		}

		Debug.Log("Mudando turno. Agora é turno do " + (isPlayerTurn ? "Jogador" : "Oponente"));

		UpdateSlotHighlight();
	}

	public void ResetTurn()
	{
		currentTurnIndex = 0;
		isPlayerTurn = true;
		UpdateSlotHighlight();
	}

	public bool IsOpponentTurn()
	{
		return !isPlayerTurn;
	}	

	public void UpdateSlotHighlight()
	{
		foreach (Transform zone in playerDropZones)
		{
			CardDropZone dropZone = zone.GetComponent<CardDropZone>();
			if (dropZone != null)
			{
				dropZone.ToggleHighlight(false);
			}
		}

		foreach (Transform zone in opponentDropZones)
		{
			CardDropZone dropZone = zone.GetComponent<CardDropZone>();
			if (dropZone != null)
			{
				dropZone.ToggleHighlight(false);
			}
		}

		Transform[] activeDropZones = isPlayerTurn ? playerDropZones : opponentDropZones;
		if (currentTurnIndex < activeDropZones.Length)
		{
			CardDropZone nextDropZone = activeDropZones[currentTurnIndex].GetComponent<CardDropZone>();
			if (nextDropZone != null)
			{
				nextDropZone.ToggleHighlight(true);
			}
		}
	}
}
