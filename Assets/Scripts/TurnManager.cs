using UnityEngine;

public class TurnManager : MonoBehaviour {
	public static TurnManager Instance;

	public Transform[] dropZones;
	private int currentTurnIndex = 0;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	public Transform GetExpectedDropZone() {
		if (currentTurnIndex >= dropZones.Length) {
			return null;
		}
		return dropZones[currentTurnIndex];
	}

	public void AdvanceTurn() {
		if (currentTurnIndex < dropZones.Length - 1) {
			currentTurnIndex++;
		}
		else {
			ResetTurn();
		}
	}

	public void ResetTurn() {
		currentTurnIndex = 0;
	}
}
