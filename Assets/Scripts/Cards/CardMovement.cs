using AxolotlProductions;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	private RectTransform rectTransform;
	private Canvas canvas;
	private Vector2 originalLocalPointerPosition;
	private Vector3 originalPanelLocalPosition;
	private Vector3 originalScale;
	private Quaternion originalRotation;
	private Vector3 originalPosition;
	private Collider2D col;

	[SerializeField] private float selectScale = 1.1f;
	[SerializeField] private GameObject gloweffect;
	[SerializeField] private float lerpFactor = 0.1f;

	private int currentState = 0;
	private CardDisplay cardDisplay;
	private bool isLocked = false;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		originalScale = rectTransform.localScale;
		originalPosition = rectTransform.localPosition;
		originalRotation = rectTransform.localRotation;
		col = GetComponent<Collider2D>();

		cardDisplay = GetComponent<CardDisplay>();
	}

	void Update() {
		if (isLocked) return;

		switch (currentState) {
			case 1:
				HandleHoverState();
				break;
			case 2:
				HandleDragState();
				if (!Input.GetMouseButton(0)) {
					HandleDrop();
				}
				break;
		}
	}

	public void LockCardInPlace() {
		isLocked = true;
	}

	public void SetScaleForBoard() {
		rectTransform.localScale = originalScale * 0.8f;
		gloweffect.SetActive(false);
	}

	private void HandleDrop() {
		if (isLocked) return;

		col.enabled = false;
		Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
		col.enabled = true;

		if (hitCollider != null && hitCollider.TryGetComponent(out ICardDropArea cardDropArea)) {
			cardDropArea.OnCardDropped(cardDisplay);
		}
		else {
			TransitionToState0();
		}
	}

	public void TransitionToState0() {
		if (isLocked) return;

		currentState = 0;
		rectTransform.localScale = originalScale;
		rectTransform.localPosition = originalPosition;
		rectTransform.localRotation = originalRotation;
		gloweffect.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (currentState == 0 && !isLocked) {
			originalPosition = rectTransform.localPosition;
			originalRotation = rectTransform.localRotation;
			originalScale = rectTransform.localScale;
			currentState = 1;
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (currentState == 1 && !isLocked) {
			TransitionToState0();
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (currentState == 1 && !isLocked) {
			currentState = 2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canvas.GetComponent<RectTransform>(),
				eventData.position,
				eventData.pressEventCamera,
				out originalLocalPointerPosition);
			originalPanelLocalPosition = rectTransform.localPosition;
		}
	}

	public void OnDrag(PointerEventData eventData) {
		if (currentState == 2 && !isLocked) {
			Vector2 localPointerPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canvas.GetComponent<RectTransform>(),
				eventData.position,
				eventData.pressEventCamera,
				out localPointerPosition)) {
				rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, lerpFactor);
			}
		}
	}

	private void HandleHoverState() {
		if (isLocked) return;
		gloweffect.SetActive(true);
		rectTransform.localScale = originalScale * selectScale;
	}

	private void HandleDragState() {
		if (isLocked) return;
		rectTransform.localRotation = Quaternion.identity;
	}
}
