using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class HorizontalCardHolder : MonoBehaviour
{
	#region Constants
	private const float SwapTweenDuration = 0.15f;
	private const float FlipDuration = 0.25f;
	#endregion

	#region Serialized Fields
	[SerializeField] private Card selectedCard;
	[SerializeField] private Card hoveredCard;
	[SerializeField] private Transform deck;
	[SerializeField] private GameObject slotPrefab;
	[SerializeField] private bool isPlayerCardHolder = true;
	[SerializeField] private bool tweenCardReturn = true;

	[Header("Dependencies")]
	[SerializeField] private ActionSystem actionSystem;

	[Header("Spawn Settings")]
	public List<Card> cards;
	#endregion

	#region Public Properties
	public HorizontalCardHolder targetHolder;
	public GameObject SlotPrefab => slotPrefab;
	public bool IsPlayerCardHolder => isPlayerCardHolder;
	#endregion

	#region Private Fields
	private RectTransform rect;
	private bool isCrossing = false;
	#endregion

	#region Unity Methods
	protected virtual void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		HandleCardShortcuts();
		HandleCardSwapping();
	}
	#endregion

	#region Public Methods
	public void InstantiateSlotExternally()
	{
		Instantiate(slotPrefab, transform);
	}

	public void BeginDrag(Card card)
	{
		selectedCard = card;
	}

	public void EndDrag(Card card)
	{
		if (selectedCard == null) return;

		selectedCard.transform.DOLocalMove(
			selectedCard.Selected ? new Vector3(0, selectedCard.SelectionOffset, 0) : Vector3.zero,
			tweenCardReturn ? SwapTweenDuration : 0f
		).SetEase(Ease.OutBack);

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
	#endregion

	#region Private Methods
	private void HandleCardShortcuts()
	{
		if (Input.GetKeyDown(KeyCode.Delete) && hoveredCard != null && cards.Contains(hoveredCard))
		{
			actionSystem.Perform(new DestroyCardGA(hoveredCard));
		}

		if (Input.GetKeyDown(KeyCode.Space) && hoveredCard != null)
		{
			actionSystem.Perform(new FlipCardGA(hoveredCard, FlipDuration));
		}
	}

	private void HandleCardSwapping()
	{
		if (selectedCard == null || isCrossing) return;

		for (int i = 0; i < cards.Count; i++)
		{
			bool selectedLeftOfCard = selectedCard.transform.position.x > cards[i].transform.position.x;
			bool selectedRightOfCard = selectedCard.transform.position.x < cards[i].transform.position.x;

			if (selectedLeftOfCard && selectedCard.ParentIndex() < cards[i].ParentIndex())
			{
				actionSystem.Perform(new SwapCardGA(selectedCard, cards[i], transform));
				break;
			}

			if (selectedRightOfCard && selectedCard.ParentIndex() > cards[i].ParentIndex())
			{
				actionSystem.Perform(new SwapCardGA(selectedCard, cards[i], transform));
				break;
			}
		}
	}
	#endregion
}