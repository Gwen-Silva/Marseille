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
	public HorizontalCardHolder targetHolder;
	[SerializeField] private bool isPlayerCardHolder = true;
	public GameObject SlotPrefab => slotPrefab;
	public bool IsPlayerCardHolder => isPlayerCardHolder;
	private RectTransform rect;

	[Header("Spawn Settings")]
	public List<Card> cards;

	bool isCrossing = false;
	[SerializeField] private bool tweenCardReturn = true;

	protected virtual void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	public void InstantiateSlotExternally()
	{
		GameObject slotGO = Instantiate(slotPrefab, transform);
	}

	public void BeginDrag(Card card)
	{
		selectedCard = card;
	}


	public void EndDrag(Card card)
	{
		if (selectedCard == null)
			return;

		selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

		rect.sizeDelta += Vector2.right;
		rect.sizeDelta -= Vector2.right;

		selectedCard = null;

	}

	public void CardPointerEnter(Card card)
	{
		hoveredCard = card;
	}

	public void CardPointerExit(Card card)
	{
		hoveredCard = null;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			if (hoveredCard != null && cards.Contains(hoveredCard))
			{
				ActionSystem.Instance.Perform(new DestroyCardGA(hoveredCard));
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (hoveredCard != null)
			{
				ActionSystem.Instance.Perform(new FlipCardGA(hoveredCard, 0.25f));
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
