using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerHand;
	[SerializeField] private HorizontalCardHolder opponentHand;
	public HorizontalCardHolder PlayerHand => playerHand;
	public HorizontalCardHolder OpponentHand => opponentHand;
	public List<CardData> playerDeck = new();
	public List<CardData> opponentDeck = new();
	[SerializeField] private GameObject cardSlotPrefab;

	private void Start()
	{
		ActionSystem.Instance.Perform(new GenerateDecksGA());
	}
}
