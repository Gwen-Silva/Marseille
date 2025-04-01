using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardVisual : MonoBehaviour
{
	#region Constants
	private const float ShadowOffset = 20f;
	private const float IndexTiltScale = 0.2f;
	private const float RotationClamp = 60f;
	private const float RotationLerpSpeed = 25f;
	private const float ShadowFadeAlpha = 0.6f;
	private const float PulseScaleIntensity = 0.15f;
	private const float PulseDuration = 0.5f;
	#endregion

	#region Serialized Fields
	[Header("Card")]
	public Card parentCard;

	[Header("References")]
	[SerializeField] private Transform shakeParent;
	[SerializeField] private Transform tiltParent;
	[SerializeField] private GameObject flipParent;
	[SerializeField] private Image auraImage;
	[SerializeField] private Transform visualShadow;

	[Header("Follow Parameters")]
	[SerializeField] private float followSpeed = 30f;

	[Header("Rotation Parameters")]
	[SerializeField] private float rotationAmount = 20f;
	[SerializeField] private float rotationSpeed = 20f;
	[SerializeField] private float autoTiltAmount = 30f;
	[SerializeField] private float manualTiltAmount = 20f;
	[SerializeField] private float tiltSpeed = 20f;

	[Header("Scale Parameters")]
	[SerializeField] private bool scaleAnimations = true;
	[SerializeField] private float scaleOnHover = 1.15f;
	[SerializeField] private float scaleOnSelect = 1.25f;
	[SerializeField] private float scaleTransition = 0.15f;
	[SerializeField] private Ease scaleEase = Ease.OutBack;

	[Header("Select Parameters")]
	[SerializeField] private float selectPunchAmount = 20f;

	[Header("Hover Parameters")]
	[SerializeField] private float hoverPunchAngle = 5f;
	[SerializeField] private float hoverTransition = 0.15f;

	[Header("Swap Parameters")]
	[SerializeField] private bool swapAnimations = true;
	[SerializeField] private float swapRotationAngle = 30f;
	[SerializeField] private float swapTransition = 0.15f;
	[SerializeField] private int swapVibrato = 5;

	[Header("Curve")]
	[SerializeField] private CurveParameters curve;
	#endregion

	#region Private Fields
	private bool initialized;
	private Vector3 rotationDelta;
	private int savedIndex;
	private Vector3 movementDelta;
	private Canvas canvas;
	private Canvas shadowCanvas;
	private Vector2 shadowDistance;
	#endregion

	#region Properties
	public GameObject FlipParent => flipParent;
	public bool isFlipped = false;
	#endregion

	private void Start()
	{
		shadowDistance = visualShadow.localPosition;
	}

	public void Initialize(Card target, int index = 0)
	{
		parentCard = target;
		canvas = GetComponent<Canvas>();
		shadowCanvas = visualShadow.GetComponent<Canvas>();

		parentCard.PointerEnterEvent.AddListener(PointerEnter);
		parentCard.PointerExitEvent.AddListener(PointerExit);
		parentCard.BeginDragEvent.AddListener(BeginDrag);
		parentCard.EndDragEvent.AddListener(EndDrag);
		parentCard.PointerDownEvent.AddListener(PointerDown);
		parentCard.PointerUpEvent.AddListener(PointerUp);
		parentCard.SelectEvent.AddListener(Select);

		initialized = true;
	}

	public void UpdateIndex(int length)
	{
		transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
	}

	private void Update()
	{
		if (!initialized || parentCard == null) return;

		HandPositioning();
		SmoothFollow();
		FollowRotation();
		CardTilt();
	}

	private void HandPositioning()
	{
		float normalizedIndex = parentCard.NormalizedPosition();
		float curveX = parentCard.isPlayerCard ? normalizedIndex : 1f - normalizedIndex;

		float baseYOffset = (curve.positioning.Evaluate(curveX) * curve.positioningInfluence) * parentCard.SiblingAmount();
		float curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : baseYOffset;
		curveYOffset = parentCard.isPlayerCard ? curveYOffset : -curveYOffset;

		movementDelta.y = curveYOffset;
		rotationDelta.z = curve.rotation.Evaluate(curveX);
	}

	private void SmoothFollow()
	{
		Vector3 verticalOffset = Vector3.up * (parentCard.isDragging ? 0 : movementDelta.y);
		transform.position = Vector3.Lerp(transform.position, parentCard.transform.position + verticalOffset, followSpeed * Time.deltaTime);
	}

	private void FollowRotation()
	{
		Vector3 movement = transform.position - parentCard.transform.position;
		movementDelta = Vector3.Lerp(movementDelta, movement, RotationLerpSpeed * Time.deltaTime);
		Vector3 movementRotation = (parentCard.isDragging ? movementDelta : movement) * rotationAmount;
		rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -RotationClamp, RotationClamp));
	}

	private void CardTilt()
	{
		savedIndex = parentCard.isDragging ? savedIndex : parentCard.ParentIndex();
		float sine = Mathf.Sin(Time.time + savedIndex) * (parentCard.isHovering ? IndexTiltScale : 1);
		float cosine = Mathf.Cos(Time.time + savedIndex) * (parentCard.isHovering ? IndexTiltScale : 1);

		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float tiltX = parentCard.isHovering ? (offset.y * -manualTiltAmount) : 0;
		float tiltY = parentCard.isHovering ? (offset.x * manualTiltAmount) : 0;
		float tiltZ = parentCard.isDragging ? tiltParent.eulerAngles.z : (rotationDelta.z * (curve.rotationInfluence * parentCard.SiblingAmount()));

		float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
		float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
		float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, (tiltSpeed / 2f) * Time.deltaTime);

		Vector3 newRotation = new Vector3(lerpX, lerpY, lerpZ);

		if (!float.IsNaN(newRotation.x) && !float.IsNaN(newRotation.y) && !float.IsNaN(newRotation.z))
		{
			tiltParent.eulerAngles = newRotation;
		}
	}

	private void Select(Card card, bool state)
	{
		DOTween.Kill(2, true);
		shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount * (state ? 1 : 0), scaleTransition, 10, 1);
		shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle / 2), hoverTransition, 20, 1).SetId(2);

		if (scaleAnimations)
			transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
	}

	public void Swap(float dir = 1f)
	{
		if (!swapAnimations) return;

		DOTween.Kill(2, true);
		shakeParent.DOPunchRotation(Vector3.forward * swapRotationAngle * dir, swapTransition, swapVibrato, 1).SetId(3);
	}

	private void BeginDrag(Card card)
	{
		if (scaleAnimations)
			transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

		canvas.overrideSorting = true;
	}

	private void EndDrag(Card card)
	{
		canvas.overrideSorting = false;
		transform.DOScale(1f, scaleTransition).SetEase(scaleEase);
	}

	private void PointerEnter(Card card)
	{
		if (scaleAnimations)
			transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

		DOTween.Kill(2, true);
		shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
	}

	private void PointerExit(Card card)
	{
		if (!parentCard.wasDragged)
			transform.DOScale(1f, scaleTransition).SetEase(scaleEase);
	}

	private void PointerUp(Card card, bool longPress)
	{
		if (scaleAnimations)
			transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);

		canvas.overrideSorting = false;
		visualShadow.localPosition = shadowDistance;
		shadowCanvas.overrideSorting = true;
	}

	private void PointerDown(Card card)
	{
		if (scaleAnimations)
			transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

		visualShadow.localPosition += -Vector3.up * ShadowOffset;
		shadowCanvas.overrideSorting = false;
	}

	public void PulseEffect(Color color)
	{
		Vector3 originalScale = transform.localScale;
		Vector3 targetScale = originalScale * (1f + PulseScaleIntensity);

		Sequence scalePulse = DOTween.Sequence();
		scalePulse.Append(transform.DOScale(targetScale, PulseDuration).SetEase(Ease.OutQuad));
		scalePulse.Append(transform.DOScale(originalScale, PulseDuration).SetEase(Ease.InQuad));

		if (auraImage != null)
		{
			auraImage.color = new Color(color.r, color.g, color.b, 0f);
			auraImage.gameObject.SetActive(true);

			Sequence auraPulse = DOTween.Sequence();
			auraPulse.Append(auraImage.DOFade(ShadowFadeAlpha, PulseDuration));
			auraPulse.Append(auraImage.DOFade(0f, PulseDuration));
			auraPulse.OnComplete(() => auraImage.gameObject.SetActive(false));
		}
	}

	public void PulseNegativeEffect()
	{
		Vector3 originalScale = transform.localScale;
		Vector3 targetScale = originalScale * (1f - PulseScaleIntensity);

		Sequence scalePulse = DOTween.Sequence();
		scalePulse.Append(transform.DOScale(targetScale, PulseDuration).SetEase(Ease.InQuad));
		scalePulse.Append(transform.DOScale(originalScale, PulseDuration).SetEase(Ease.OutQuad));

		if (auraImage != null)
		{
			auraImage.color = new Color(0f, 0f, 0f, 0f);
			auraImage.gameObject.SetActive(true);

			Sequence auraPulse = DOTween.Sequence();
			auraPulse.Append(auraImage.DOFade(ShadowFadeAlpha, PulseDuration));
			auraPulse.Append(auraImage.DOFade(0f, PulseDuration));
			auraPulse.OnComplete(() => auraImage.gameObject.SetActive(false));
		}
	}
}