using UnityEngine;

public class TurnManager : MonoBehaviour {
	public static TurnManager Instance;

	public Transform[] playerDropZones;
	public Transform[] opponentDropZones;
	private int currentTurnIndex = 0;
	private bool isPlayerTurn = true;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		UpdateSlotHighlight();
	}



	public Transform GetExpectedDropZone() {
		if (isPlayerTurn) {
			if (currentTurnIndex < playerDropZones.Length) {
				return playerDropZones[currentTurnIndex];
			}
		}
		else {
			if (currentTurnIndex < opponentDropZones.Length) {
				return opponentDropZones[currentTurnIndex];
			}
		}
		return null;
	}


	public void AdvanceTurn() {
		Transform[] activeDropZones = isPlayerTurn ? playerDropZones : opponentDropZones;

		if (currentTurnIndex < activeDropZones.Length - 1) {
			currentTurnIndex++;
		}
		else {
			currentTurnIndex = 0;
			isPlayerTurn = !isPlayerTurn;
		}

		Debug.Log("Mudando turno. Agora é turno do " + (isPlayerTurn ? "Jogador" : "Oponente"));

		UpdateSlotHighlight();
	}


	public void ResetTurn() {
		currentTurnIndex = 0;
		isPlayerTurn = true;
		UpdateSlotHighlight();
	}

	public bool IsOpponentTurn() {
		return !isPlayerTurn;
	}

	public void UpdateSlotHighlight() {
		foreach (Transform zone in playerDropZones) {
			CardDropZone dropZone = zone.GetComponent<CardDropZone>();
			if (dropZone != null) {
				dropZone.ToggleHighlight(false);
			}
		}

		foreach (Transform zone in opponentDropZones) {
			CardDropZone dropZone = zone.GetComponent<CardDropZone>();
			if (dropZone != null) {
				dropZone.ToggleHighlight(false);
			}
		}

		Transform[] activeDropZones = isPlayerTurn ? playerDropZones : opponentDropZones;
		if (currentTurnIndex < activeDropZones.Length) {
			CardDropZone nextDropZone = activeDropZones[currentTurnIndex].GetComponent<CardDropZone>();
			if (nextDropZone != null) {
				nextDropZone.ToggleHighlight(true);
			}
		}
	}
}
