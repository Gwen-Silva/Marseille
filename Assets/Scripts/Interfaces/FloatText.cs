using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatText : MonoBehaviour
{
	#region Constants

	private const float DEFAULT_FONT_SIZE = 50f;
	private const float SCALE_POP_DURATION = 0.3f;
	private static readonly Ease SCALE_EASE = Ease.OutBack;
	private static readonly Ease MOVE_EASE = Ease.OutCubic;
	private static readonly Ease FADE_EASE = Ease.InOutQuad;

	#endregion

	#region Serialized Fields

	[SerializeField] private TMP_Text text;
	[SerializeField] private float floatHeight = 1.0f;
	[SerializeField] private float duration = 1.5f;
	[SerializeField] private float scalePop = 1.3f;
	[SerializeField] private float fadeDuration = 0.5f;

	#endregion

	#region Private Fields

	private CanvasGroup canvasGroup;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Initializes the floating text with a numeric value (positive or negative).
	/// </summary>
	public void Initialize(int amount, bool isHeal)
	{
		string prefix = isHeal ? "+" : "-";
		Color color = isHeal ? Color.green : Color.red;
		SetupText(prefix + Mathf.Abs(amount), color, DEFAULT_FONT_SIZE);
	}

	/// <summary>
	/// Initializes the floating text with a custom message, color and optional font size.
	/// </summary>
	public void InitializeCustom(string message, Color color, float fontSize = DEFAULT_FONT_SIZE)
	{
		SetupText(message, color, fontSize);
	}

	#endregion

	#region Private Methods

	private void SetupText(string message, Color color, float fontSize)
	{
		text.text = message;
		text.color = color;
		text.fontSize = fontSize;

		transform.localScale = Vector3.zero;

		transform.DOScale(Vector3.one * scalePop, SCALE_POP_DURATION).SetEase(SCALE_EASE);
		transform.DOMoveY(transform.position.y + floatHeight, duration).SetEase(MOVE_EASE);

		canvasGroup.DOFade(0, fadeDuration)
			.SetEase(FADE_EASE)
			.SetDelay(duration - fadeDuration)
			.OnComplete(() => Destroy(gameObject));
	}

	#endregion
}
