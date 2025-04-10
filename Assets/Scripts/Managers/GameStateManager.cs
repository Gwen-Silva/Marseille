using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
	public static GameStateManager Instance { get; private set; }

	public GameState CurrentState { get; private set; }

	public static event Action<GameState> OnGameStateChanged;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		InitializeState();
	}

	private void InitializeState()
	{
		if (CurrentState != default)
			return;

		string sceneName = SceneManager.GetActiveScene().name;

		if (sceneName == "Gameplay")
		{
			CurrentState = GameState.Playing;
			Debug.Log("[GameStateManager] Cena Gameplay detectada. Estado inicial definido como Playing.");
		}
		else if (sceneName == "Main Menu")
		{
			CurrentState = GameState.MainMenu;
			Debug.Log("[GameStateManager] Cena Main Menu detectada. Estado inicial definido como MainMenu.");
		}
	}

	public void ChangeState(GameState newState)
	{
		if (newState == CurrentState)
		{
			Debug.Log($"[GameStateManager] Estado já é {newState}, ignorando troca.");
			return;
		}

		Debug.Log($"[GameStateManager] Mudando de estado {CurrentState} para {newState}");
		CurrentState = newState;
		OnGameStateChanged?.Invoke(newState);

		switch (newState)
		{
			case GameState.MainMenu:
				Debug.Log("[GameStateManager] Carregando cena MainMenu...");
				SceneLoaderManager.Instance?.LoadScene("Main Menu");
				break;

			case GameState.Playing:
				if (SceneManager.GetActiveScene().name != "Gameplay")
				{
					Debug.Log("[GameStateManager] Carregando cena Gameplay...");
					SceneLoaderManager.Instance?.LoadScene("Gameplay");
				}
				break;

			case GameState.Paused:
				Debug.Log("[GameStateManager] Estado pausado");
				break;

			case GameState.GameOver:
				Debug.Log("[GameStateManager] Game Over.");
				break;
		}
	}

	public void TogglePause()
	{
		Debug.Log("[GameStateManager] Alternando pausa...");
		if (CurrentState == GameState.Playing)
			ChangeState(GameState.Paused);
		else if (CurrentState == GameState.Paused)
			ChangeState(GameState.Playing);
	}
}
