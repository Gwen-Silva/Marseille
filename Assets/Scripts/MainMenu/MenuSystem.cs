using UnityEngine;

public class MenuSystem : MonoBehaviour
{
	[SerializeField] private string gameOptionsMenu = "Menu Options";

	public void StartGame()
	{
		GameStateManager.Instance?.ChangeState(GameState.Playing);
	}

	public void ReturnToMainMenu()
	{
		GameStateManager.Instance?.ChangeState(GameState.MainMenu);
	}

	public void Options()
	{
		SceneLoaderManager.Instance?.LoadScene(gameOptionsMenu);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
