using UnityEngine;

public class PauseButtonUI : MonoBehaviour
{
	[SerializeField] private PauseHandler pauseHandler;

	public void TogglePause()
	{
		var gameState = GameStateManager.Instance.CurrentState;

		if (gameState == GameState.Playing)
		{
			GameStateManager.Instance.TogglePause();
		}
		else if (gameState == GameState.Paused)
		{
			if (pauseHandler != null)
			{
				pauseHandler.RequestResume();
			}
			else
			{
				GameStateManager.Instance.TogglePause();
			}
		}
		else
		{
			Debug.LogWarning("[PauseButtonUI] Não é possível pausar/despausar fora da gameplay.");
		}
	}
}
