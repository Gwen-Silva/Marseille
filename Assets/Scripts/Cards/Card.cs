
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
	private Canvas canvas;
	private Image imageComponent;
	[SerializeField] private bool instantiateVisual = true;
	[HideInInspector] public CardData cardData;
	private VisualCardsHandler visualHandler;
	private Vector3 offset;

	[Header("Movement")]
	[SerializeField] private float moveSpeedLimit = 50;

	[Header("Selection")]
	public bool selected;
	public float selectionOffset = 50;
	private float pointerDownTime;
	private float pointerUpTime;

	[Header("Visual")]
	[SerializeField] private GameObject cardVisualPrefab;
	[HideInInspector] public CardVisual cardVisual;

	[Header("States")]
	public bool isHovering;
	public bool isDragging;
	[HideInInspector] public bool wasDragged;

	[Header("Events")]
	[HideInInspector] public UnityEvent<Card> PointerEnterEvent;
	[HideInInspector] public UnityEvent<Card> PointerExitEvent;
	[HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
	[HideInInspector] public UnityEvent<Card> PointerDownEvent;
	[HideInInspector] public UnityEvent<Card> BeginDragEvent;
	[HideInInspector] public UnityEvent<Card> EndDragEvent;
	[HideInInspector] public UnityEvent<Card, bool> SelectEvent;

	public bool isPlayerCard = true;

	void Start()
	{
		canvas = GetComponentInParent<Canvas>();
		imageComponent = GetComponent<Image>();

		if (!instantiateVisual)
			return;

		string tagToUse = isPlayerCard ? "PlayerVisualHandler" : "OpponentVisualHandler";
		GameObject handlerGO = GameObject.FindGameObjectWithTag(tagToUse);
		visualHandler = handlerGO ? handlerGO.GetComponent<VisualCardsHandler>() : null;

		cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
		cardVisual.Initialize(this);
		CardDisplay display = cardVisual.GetComponent<CardDisplay>();
		if (display != null)
		{
			display.OwnerCard = this;
			display.SetupDisplay(cardData);
		}
	}

	void Update()
	{
		ClampPosition();

		if (isDragging)
		{
			Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
			Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
			Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
			transform.Translate(velocity * Time.deltaTime);
		}
	}

	void ClampPosition()
	{
		Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		Vector3 clampedPosition = transform.position;
		clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
		clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
		transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		BeginDragEvent.Invoke(this);

		if (selected)
		{
			selected = false;
			SelectEvent.Invoke(this, false);
			transform.localPosition = Vector3.zero;
		}

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		offset = Vector2.zero;
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		isDragging = true;
		canvas.GetComponent<GraphicRaycaster>().enabled = false;
		imageComponent.raycastTarget = false;

		wasDragged = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		EndDragEvent.Invoke(this);
		isDragging = false;
		canvas.GetComponent<GraphicRaycaster>().enabled = true;
		imageComponent.raycastTarget = true;

		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raycastResults);

		foreach (var result in raycastResults)
		{
			var dropZone = result.gameObject.GetComponentInParent<ICardDropArea>();
			if (dropZone != null)
			{
				dropZone.OnCardDropped(this);
				StartCoroutine(FrameWait());
				return;
			}
		}

		StartCoroutine(FrameWait());

		IEnumerator FrameWait()
		{
			yield return new WaitForEndOfFrame();
			wasDragged = false;
		}
	}


	public void OnPointerEnter(PointerEventData eventData)
	{
		PointerEnterEvent.Invoke(this);
		isHovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		PointerExitEvent.Invoke(this);
		isHovering = false;
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;

		PointerDownEvent.Invoke(this);
		pointerDownTime = Time.time;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;

		pointerUpTime = Time.time;

		PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

		if (pointerUpTime - pointerDownTime > .2f)
			return;

		if (wasDragged)
			return;

		ActionSystem.Instance.Perform(new SelectCardGA(this));
	}

	public void Deselect()
	{
		ActionSystem.Instance.Perform(new DeselectCardGA(this));
	}

	public int SiblingAmount()
	{
		return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
	}

	public int ParentIndex()
	{
		return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
	}

	public float NormalizedPosition()
	{
		return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
	}

	private void OnDestroy()
	{
		if (ActionSystem.Instance != null)
		{
			ActionSystem.Instance.Perform(new DestroyCardGA(this));
		}
	}

	public void DisableInteraction()
	{
		PointerEnterEvent.RemoveAllListeners();
		PointerExitEvent.RemoveAllListeners();
		BeginDragEvent.RemoveAllListeners();
		EndDragEvent.RemoveAllListeners();
		PointerDownEvent.RemoveAllListeners();
		PointerUpEvent.RemoveAllListeners();
		SelectEvent.RemoveAllListeners();

		isHovering = false;
		PointerExitEvent.Invoke(this);
	}
}
