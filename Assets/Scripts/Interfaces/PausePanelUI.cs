using UnityEngine;

public class PausePanelUI : MonoBehaviour
{
	private void OnEnable()
	{
		GameStateManager.OnGameStateChanged += HandleStateChange;
	}

	private void OnDisable()
	{
		GameStateManager.OnGameStateChanged -= HandleStateChange;
	}

	private void HandleStateChange(GameState newState)
	{
		gameObject.SetActive(newState == GameState.Paused);
	}
}
