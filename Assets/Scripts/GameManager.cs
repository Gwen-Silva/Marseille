using UnityEngine;
using System.Collections.Generic;
using AxolotlProductions;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<CardData> cardDatas;
    [SerializeField] private CardView cardview;
    private List<Card> deck;
	private void Start() {
		deck = new();
		for (int i = 0; i < 10; i++) {
			CardData data = cardDatas[Random.Range(0, cardDatas.Count)];
			Card card = new(data);
			deck.Add(card);
		}
	}
	public void DrawCard() {
		Card drawnCard = deck[Random.Range(0, deck.Count)];
		deck.Remove(drawnCard);
		CardView view = Instantiate(cardview);
		view.Setup(drawnCard);
	}
}
