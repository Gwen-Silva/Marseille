using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoState<CardSystem>
{
	#region Constants

	private const float DELAY_VERY_SHORT = 0.1f;
	private const float DELAY_SHORT = 0.2f;
	private const float DELAY_MEDIUM = 0.3f;
	private const float DELAY_LONG = 0.4f;
	private const float DELAY_VERY_LONG = 0.6f;
	private const float DEFAULT_DELAY = DELAY_MEDIUM;

	#endregion

	#region Serialized Fields

	[SerializeField] private HorizontalCardHolder playerHand;
	[SerializeField] private HorizontalCardHolder opponentHand;
	[SerializeField] private GameObject cardSlotPrefab;

	#endregion

	#region Public Properties

	public HorizontalCardHolder PlayerHand => playerHand;
	public HorizontalCardHolder OpponentHand => opponentHand;

	#endregion

	#region Internal State

	public List<CardData> playerDeck = new();
	public List<CardData> opponentDeck = new();

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<WaitGA>(WaitPerformer);
		ActionSystem.AttachPerformer<InitializeGameplayGA>(InitializeGameplayPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<WaitGA>();
		ActionSystem.DetachPerformer<InitializeGameplayGA>();
	}

	private void Start()
	{
		ActionSystem.Shared?.Perform(new InitializeGameplayGA());
	}

	#endregion

	#region Performers

	private IEnumerator WaitPerformer(WaitGA ga)
	{
		yield return new WaitForSeconds(GetDelayValue(ga.DelayLevel));
	}

	private IEnumerator InitializeGameplayPerformer(InitializeGameplayGA ga)
	{
		yield return null;
	}

	#endregion

	#region Helpers

	private float GetDelayValue(DelayType delayType)
	{
		return delayType switch
		{
			DelayType.VeryShort => DELAY_VERY_SHORT,
			DelayType.Short => DELAY_SHORT,
			DelayType.Medium => DELAY_MEDIUM,
			DelayType.Long => DELAY_LONG,
			DelayType.VeryLong => DELAY_VERY_LONG,
			_ => DEFAULT_DELAY
		};
	}

	#endregion

	#region Reset System

	public static void ResetGame()
	{
		Debug.Log("[GameSystem] Resetando o jogo...");

		ActionSystem.Clear();

		TurnSystem.Reset();
		DeckSystem.Reset();
		HealthSystem.Reset();
		EffectSystem.Reset();

		foreach (var card in GameObject.FindObjectsByType<Card>(FindObjectsSortMode.None))
		{
			Destroy(card.gameObject);
		}

		Debug.Log("[GameSystem] Jogo resetado com sucesso.");
	}

	#endregion
}
