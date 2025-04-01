using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
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
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<WaitGA>();
	}

	private void Start()
	{
		ActionSystem.Instance.Perform(new GenerateDecksGA());
	}

	#endregion

	#region Performers

	/// <summary>
	/// Waits for a specified delay duration based on the DelayType provided.
	/// </summary>
	private IEnumerator WaitPerformer(WaitGA ga)
	{
		yield return new WaitForSeconds(GetDelayValue(ga.DelayLevel));
	}

	#endregion

	#region Helpers

	/// <summary>
	/// Returns the corresponding float delay time for a given DelayType enum.
	/// </summary>
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
}
