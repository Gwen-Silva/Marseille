using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class HorizontalCardHolder : MonoBehaviour
{

	[SerializeField] private Card selectedCard;
	[SerializeReference] private Card hoveredCard;

	[SerializeField] private Transform deck;
	[SerializeField] private GameObject slotPrefab;
	[SerializeField] private bool isPlayerCardHolder = true;
	private RectTransform rect;

	[Header("Spawn Settings")]
	[SerializeField] private int cardsToSpawn = 8;
	public List<Card> cards;

	bool isCrossing = false;
	[SerializeField] private bool tweenCardReturn = true;

	protected virtual void Start()
	{
		rect = GetComponent<RectTransform>();
		SpawnCards(cardsToSpawn);
	}

	public void SpawnCards(int amount)
	{
		StartCoroutine(SpawnRoutine(amount));
	}

	private IEnumerator SpawnRoutine(int amount)
	{
		if (cards == null)
			cards = new List<Card>();

		for (int i = 0; i < amount; i++)
		{
			GameObject slotGO = Instantiate(slotPrefab, transform);
			Card card = slotGO.GetComponentInChildren<Card>();
			card.isPlayerCard = isPlayerCardHolder;

			cards.Add(card);

			card.PointerEnterEvent.AddListener(CardPointerEnter);
			card.PointerExitEvent.AddListener(CardPointerExit);
			card.BeginDragEvent.AddListener(BeginDrag);
			card.EndDragEvent.AddListener(EndDrag);
			card.name = cards.Count.ToString();

			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSecondsRealtime(0.1f);

		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i].cardVisual != null)
				cards[i].cardVisual.UpdateIndex(transform.childCount);
		}
	}

	public void InstantiateSlotExternally()
	{
		GameObject slotGO = Instantiate(slotPrefab, transform);
	}

	private void BeginDrag(Card card)
	{
		selectedCard = card;
	}


	void EndDrag(Card card)
	{
		if (selectedCard == null)
			return;

		selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

		rect.sizeDelta += Vector2.right;
		rect.sizeDelta -= Vector2.right;

		selectedCard = null;

	}

	void CardPointerEnter(Card card)
	{
		hoveredCard = card;
	}

	void CardPointerExit(Card card)
	{
		hoveredCard = null;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			if (hoveredCard != null)
			{
				ActionSystem.Instance.Perform(new DestroyCardGA(hoveredCard));
			}
		}

		if (selectedCard == null || isCrossing)
			return;

		for (int i = 0; i < cards.Count; i++)
		{
			if (selectedCard.transform.position.x > cards[i].transform.position.x && selectedCard.ParentIndex() < cards[i].ParentIndex())
			{
				ActionSystem.Instance.Perform(new SwapCardGA(selectedCard, cards[i], transform));
				break;
			}

			if (selectedCard.transform.position.x < cards[i].transform.position.x && selectedCard.ParentIndex() > cards[i].ParentIndex())
			{
				ActionSystem.Instance.Perform(new SwapCardGA(selectedCard, cards[i], transform));
				break;
			}
		}
	}
}
