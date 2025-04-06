using UnityEngine;
using UnityEngine.UI;

public class PauseInputBlocker : MonoBehaviour
{
	private Card[] allCards;

	private void OnEnable()
	{
		GameStateManager.OnGameStateChanged += HandleStateChange;
		allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
	}

	private void OnDisable()
	{
		GameStateManager.OnGameStateChanged -= HandleStateChange;
	}

	private void HandleStateChange(GameState newState)
	{
		bool block = newState == GameState.Paused;
		allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

		foreach (var card in allCards)
		{
			card.enabled = !block;

			var selectable = card.GetComponent<Selectable>();
			if (selectable != null)
				selectable.enabled = !block;
		}
	}
}
