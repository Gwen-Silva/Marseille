using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ExtensionMethods;

public class GameSystem : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerHand;
	[SerializeField] private HorizontalCardHolder opponentHand;
	public HorizontalCardHolder PlayerHand => playerHand;
	public HorizontalCardHolder OpponentHand => opponentHand;
	public List<CardData> playerDeck = new();
	public List<CardData> opponentDeck = new();
	[SerializeField] private GameObject cardSlotPrefab;

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

	private IEnumerator WaitPerformer(WaitGA ga)
	{
		yield return new WaitForSeconds(GetDelayValue(ga.DelayLevel));
	}

	private float GetDelayValue(DelayType type)
	{
		return type switch
		{
			DelayType.VeryShort => 0.1f,
			DelayType.Short => 0.2f,
			DelayType.Medium => 0.3f,
			DelayType.Long => 0.4f,
			DelayType.VeryLong => 0.6f,
			_ => 0.3f
		};
	}
}
