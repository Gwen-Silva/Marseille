using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class Card : MonoBehaviour,
	IDragHandler,
	IBeginDragHandler,
	IEndDragHandler,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerUpHandler,
	IPointerDownHandler
{
	#region Constants
	private const float MoveSpeedLimit = 50f;
	private const float DefaultSelectionOffset = 50f;
	private const float HoldThreshold = 0.2f;
	#endregion

	#region Serialized Fields
	[SerializeField] private bool instantiateVisual = true;
	[SerializeField] private GameObject cardVisualPrefab;
	#endregion

	#region Public Fields
	[HideInInspector] public CardData cardData;
	[HideInInspector] public CardVisual cardVisual;
	[HideInInspector] public bool isPlayerCard = true;
	[HideInInspector] public bool wasDragged;
	public bool isHovering;
	public bool isDragging;
	public bool selected;
	private float _selectionOffset = DefaultSelectionOffset;
	#endregion

	#region Properties
	public bool Selected { get => selected; set => selected = value; }
	public float SelectionOffset => _selectionOffset;
	#endregion

	#region Events
	[HideInInspector] public UnityEvent<Card> PointerEnterEvent;
	[HideInInspector] public UnityEvent<Card> PointerExitEvent;
	[HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
	[HideInInspector] public UnityEvent<Card> PointerDownEvent;
	[HideInInspector] public UnityEvent<Card> BeginDragEvent;
	[HideInInspector] public UnityEvent<Card> EndDragEvent;
	[HideInInspector] public UnityEvent<Card, bool> SelectEvent;
	#endregion

	#region Private Fields
	private Canvas parentCanvas;
	private Image imageComponent;
	private VisualCardsHandler visualHandler;
	private Vector3 dragOffset;
	private float pointerDownTime;
	private float pointerUpTime;
	#endregion

	#region Unity Methods
	void Start()
	{
		parentCanvas = GetComponentInParent<Canvas>();
		imageComponent = GetComponent<Image>();

		if (!instantiateVisual) return;

		string handlerTag = isPlayerCard ? "PlayerVisualHandler" : "OpponentVisualHandler";
		GameObject handlerGO = GameObject.FindGameObjectWithTag(handlerTag);
		visualHandler = handlerGO ? handlerGO.GetComponent<VisualCardsHandler>() : null;

		Transform parentTransform = visualHandler ? visualHandler.transform : parentCanvas.transform;
		cardVisual = Instantiate(cardVisualPrefab, parentTransform).GetComponent<CardVisual>();
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

		if (!isDragging) return;

		Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
		Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
		float distance = Vector2.Distance(transform.position, targetPosition);
		Vector2 velocity = direction * Mathf.Min(MoveSpeedLimit, distance / Time.deltaTime);
		transform.Translate(velocity * Time.deltaTime);
	}
	#endregion

	#region Interaction Logic
	void ClampPosition()
	{
		Vector2 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		Vector3 pos = transform.position;
		pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
		pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);
		transform.position = new Vector3(pos.x, pos.y, 0);
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

		dragOffset = Vector2.zero;
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		isDragging = true;
		wasDragged = true;

		parentCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		imageComponent.raycastTarget = false;
	}

	public void OnDrag(PointerEventData eventData) { }

	public void OnEndDrag(PointerEventData eventData)
	{
		EndDragEvent.Invoke(this);
		isDragging = false;
		parentCanvas.GetComponent<GraphicRaycaster>().enabled = true;
		imageComponent.raycastTarget = true;

		List<RaycastResult> results = new();
		EventSystem.current.RaycastAll(eventData, results);

		foreach (var result in results)
		{
			var dropZone = result.gameObject.GetComponentInParent<ICardDropArea>();
			if (dropZone != null)
			{
				dropZone.OnCardDropped(this);
				StartCoroutine(ResetDragStatus());
				return;
			}
		}

		StartCoroutine(ResetDragStatus());
	}

	IEnumerator ResetDragStatus()
	{
		yield return new WaitForEndOfFrame();
		wasDragged = false;
	}
	#endregion

	#region Pointer Events
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
		if (eventData.button != PointerEventData.InputButton.Left) return;
		PointerDownEvent.Invoke(this);
		pointerDownTime = Time.time;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		pointerUpTime = Time.time;
		bool isHold = pointerUpTime - pointerDownTime > HoldThreshold;
		PointerUpEvent.Invoke(this, isHold);
		if (isHold || wasDragged) return;
		ActionSystem.Instance.Perform(new SelectCardGA(this));
	}
	#endregion

	#region Utility
	public void Deselect()
	{
		ActionSystem.Instance.Perform(new DeselectCardGA(this));
	}

	public int SiblingAmount() => transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
	public int ParentIndex() => transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
	public float NormalizedPosition() => transform.parent.CompareTag("Slot")
		? ExtensionMethods.Remap(ParentIndex(), 0, transform.parent.parent.childCount - 1, 0, 1)
		: 0;
	#endregion

	#region Lifecycle
	private void OnDestroy()
	{
		if (ActionSystem.Instance != null)
		{
			ActionSystem.Instance.Perform(new DestroyCardGA(this));
		}
	}
	#endregion

	#region State Management
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
	#endregion
}