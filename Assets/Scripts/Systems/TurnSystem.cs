using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Controls the turn order, round progression and active slot during the game.
/// </summary>
public class TurnSystem : MonoBehaviour
{
	#region Constants

	private const float ROUND_DELAY = 0.5f;

	#endregion

	#region Serialized Fields

	[SerializeField] private List<CardDropZone> playerValueSlots;
	[SerializeField] private List<CardDropZone> playerEffectSlots;
	[SerializeField] private List<CardDropZone> opponentValueSlots;
	[SerializeField] private List<CardDropZone> opponentEffectSlots;

	[Header("Dependencies")]
	[SerializeField] private ActionSystem actionSystem;

	#endregion

	#region Public Properties

	public CardDropZone PlayerValueSlot => playerValueSlots[0];
	public CardDropZone OpponentValueSlot => opponentValueSlots[0];
	public CardDropZone PlayerEffectSlot => playerEffectSlots[0];
	public CardDropZone OpponentEffectSlot => opponentEffectSlots[0];

	public bool IsFirstRound => roundCount == 1;
	public bool IsPlayerStarting => isPlayerStarting;
	public CardDropZone CurrentActiveSlot => turnOrder[currentTurnIndex];

	#endregion

	#region Private Fields

	private int currentTurnIndex = 0;
	private int roundCount = 0;
	private bool isPlayerStarting = true;

	private List<CardDropZone> turnOrder = new();

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<AdvanceTurnGA>(AdvanceTurnPerformer);
		ActionSystem.AttachPerformer<StartGameGA>(StartGamePerformer);
		ActionSystem.AttachPerformer<ResolveRoundGA>(ResolveRoundPerformer);
		ActionSystem.AttachPerformer<ToggleTurnOwnerGA>(ToggleTurnOwnerPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<AdvanceTurnGA>();
		ActionSystem.DetachPerformer<StartGameGA>();
		ActionSystem.DetachPerformer<ResolveRoundGA>();
		ActionSystem.DetachPerformer<ToggleTurnOwnerGA>();
	}

	#endregion

	#region Performers

	/// <summary>
	/// Sets up the round and shows the round number.
	/// </summary>
	private IEnumerator StartGamePerformer(StartGameGA ga)
	{
		roundCount++;
		SetupRound();

		RoundAnnouncement announcer = FindFirstObjectByType<RoundAnnouncement>();
		announcer?.ShowRound(roundCount);

		yield return null;
	}

	/// <summary>
	/// Advances to the next slot in the turn order or resolves the round.
	/// </summary>
	private IEnumerator AdvanceTurnPerformer(AdvanceTurnGA ga)
	{
		DisableAllSlots();
		currentTurnIndex++;

		if (currentTurnIndex >= turnOrder.Count)
		{
			actionSystem.AddReaction(new ResolveRoundGA());
		}
		else
		{
			EnableSlot(turnOrder[currentTurnIndex]);
		}

		yield return null;
	}

	/// <summary>
	/// Adds a delay before resolving the round visually.
	/// </summary>
	private IEnumerator ResolveRoundPerformer(ResolveRoundGA ga)
	{
		yield return new WaitForSeconds(ROUND_DELAY);
	}

	/// <summary>
	/// Toggles which player starts the round.
	/// </summary>
	private IEnumerator ToggleTurnOwnerPerformer(ToggleTurnOwnerGA ga)
	{
		isPlayerStarting = !isPlayerStarting;
		yield return null;
	}

	#endregion

	#region Helpers

	/// <summary>
	/// Sets the turn order based on the current starting player.
	/// </summary>
	private void SetupRound()
	{
		turnOrder.Clear();

		if (isPlayerStarting)
		{
			turnOrder.Add(playerValueSlots[0]);
			turnOrder.Add(opponentValueSlots[0]);
			turnOrder.Add(playerEffectSlots[0]);
			turnOrder.Add(opponentEffectSlots[0]);
		}
		else
		{
			turnOrder.Add(opponentValueSlots[0]);
			turnOrder.Add(playerValueSlots[0]);
			turnOrder.Add(opponentEffectSlots[0]);
			turnOrder.Add(playerEffectSlots[0]);
		}

		currentTurnIndex = 0;
		EnableSlot(turnOrder[0]);
	}

	/// <summary>
	/// Enables the given slot and highlights it visually.
	/// </summary>
	private void EnableSlot(CardDropZone slot)
	{
		slot.ToggleHighlight(true);
		slot.enabled = true;
	}

	/// <summary>
	/// Disables all slots and removes their highlights.
	/// </summary>
	private void DisableAllSlots()
	{
		foreach (var slot in turnOrder)
		{
			slot.ToggleHighlight(false);
			slot.enabled = false;
		}
	}

	#endregion
}
