using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSystem : MonoBehaviour
{
	[SerializeField] private string mainMenuSceneName = "Main Menu";
	[SerializeField] private string gameSceneName = "Gameplay";
	[SerializeField] private string gameOptionsMenu = "Menu Options";

	public void StartGame()
	{
		Debug.Log("Iniciando o jogo...");
		SceneManager.LoadScene(gameSceneName);
		StartCoroutine(DelayedStartGame());
	}

	private IEnumerator DelayedStartGame()
	{
		yield return new WaitForSeconds(0.1f);

		if (ActionSystem.Instance != null)
		{
			if (GameStateManager.Instance != null)
				GameStateManager.Instance.SetState(GameState.Playing);
		}
	}

	public void ReturnToMainMenu()
	{
		Debug.Log("Voltando ao menu principal...");
		SceneManager.LoadScene(mainMenuSceneName);
	}

	public void Options()
	{
		Debug.Log("Abrindo o menu de opções...");
		SceneManager.LoadScene(gameOptionsMenu);
	}

	public void ExitGame()
	{
		Debug.Log("Saindo do jogo...");
		Application.Quit();
	}
}