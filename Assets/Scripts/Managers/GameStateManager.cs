using UnityEngine;
using System;

/// <summary>
/// Manages the current state of the game and notifies listeners of changes.
/// </summary>
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
	}

	private void Start()
	{
		SetState(GameState.Playing);
	}

	public void SetState(GameState newState)
	{
		if (newState == CurrentState) return;

		CurrentState = newState;

		OnGameStateChanged?.Invoke(newState);
	}

	/// <summary>
	/// Switches between the Playing and Paused states.
	/// </summary>
	public void TogglePause()
	{
		if (CurrentState == GameState.Playing)
		{
			SetState(GameState.Paused);
		}
		else if (CurrentState == GameState.Paused)
		{
			SetState(GameState.Playing);
		}
	}
}