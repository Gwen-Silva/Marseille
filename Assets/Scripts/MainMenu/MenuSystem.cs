using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
	[SerializeField] private string gameSceneName = "Gameplay";
	[SerializeField] private string gameOptionsMenu = "Menu Options";

	public void StartGame()
	{
		Debug.Log("Iniciando o jogo...");
		SceneManager.LoadScene(gameSceneName);
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
