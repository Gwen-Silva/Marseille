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
		}
		else if (sceneName == "Main Menu")
		{
			CurrentState = GameState.MainMenu;
		}
	}

	public void ChangeState(GameState newState)
	{
		if (newState == CurrentState)
		{
			return;
		}

		CurrentState = newState;
		OnGameStateChanged?.Invoke(newState);

		switch (newState)
		{
			case GameState.MainMenu:
				SceneLoaderManager.Instance?.LoadScene("Main Menu");
				break;

			case GameState.Playing:
				if (SceneManager.GetActiveScene().name != "Gameplay")
				{
					SceneLoaderManager.Instance?.LoadScene("Gameplay");
				}
				break;

			case GameState.Paused:
				break;

			case GameState.GameOver:
				break;
		}
	}

	public void TogglePause()
	{
		if (CurrentState == GameState.Playing)
			ChangeState(GameState.Paused);
		else if (CurrentState == GameState.Paused)
			ChangeState(GameState.Playing);
	}
}
