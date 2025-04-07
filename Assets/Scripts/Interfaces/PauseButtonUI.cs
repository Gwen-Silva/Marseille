using UnityEngine;

public class PauseButtonUI : MonoBehaviour
{
	[SerializeField] private PauseHandler pauseHandler;

	public void TogglePause()
	{
		if (GameStateManager.Instance.CurrentState == GameState.Paused)
		{
			if (pauseHandler != null)
				pauseHandler.RequestResume();
		}
		else
		{
			GameStateManager.Instance.TogglePause();
		}
	}
}
