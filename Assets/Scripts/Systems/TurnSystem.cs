using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TurnSystem : MonoBehaviour
{
	[SerializeField] private List<CardDropZone> playerValueSlots;
	[SerializeField] private List<CardDropZone> playerEffectSlots;
	public CardDropZone PlayerValueSlot => playerValueSlots[0];
	public CardDropZone OpponentValueSlot => opponentValueSlots[0];
	[SerializeField] private List<CardDropZone> opponentValueSlots;
	[SerializeField] private List<CardDropZone> opponentEffectSlots;

	private int currentTurnIndex = 0;
	private bool isPlayerStarting = true;
	public CardDropZone CurrentActiveSlot => turnOrder[currentTurnIndex];

	private List<CardDropZone> turnOrder = new();

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<AdvanceTurnGA>(AdvanceTurnPerformer);
		ActionSystem.AttachPerformer<StartGameGA>(StartGamePerformer);
		ActionSystem.AttachPerformer<ResolveRoundGA>(ResolveRoundPerformer);
	}
	private void OnDisable()
	{
		ActionSystem.DetachPerformer<AdvanceTurnGA>();
		ActionSystem.DetachPerformer<StartGameGA>();
		ActionSystem.DetachPerformer<ResolveRoundGA>();
	}

	private IEnumerator StartGamePerformer(StartGameGA ga)
	{
		SetupRound();
		yield return null;
	}

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

	private IEnumerator AdvanceTurnPerformer(AdvanceTurnGA ga)
	{
		DisableAllSlots();

		currentTurnIndex++;

		if (currentTurnIndex >= turnOrder.Count)
		{
			ActionSystem.Instance.AddReaction(new ResolveRoundGA());
		}
		else
		{
			EnableSlot(turnOrder[currentTurnIndex]);
		}

		yield return null;
	}

	private void EnableSlot(CardDropZone slot)
	{
		slot.ToggleHighlight(true);
		slot.enabled = true;
	}

	private void DisableAllSlots()
	{
		foreach (var slot in turnOrder)
		{
			slot.ToggleHighlight(false);
			slot.enabled = false;
		}
	}

	private IEnumerator ResolveRoundPerformer(ResolveRoundGA ga)
	{
		Debug.Log("[TurnSystem] Resolvendo rodada");
		yield return new WaitForSeconds(0.5f);

		isPlayerStarting = !isPlayerStarting;
	}
}
