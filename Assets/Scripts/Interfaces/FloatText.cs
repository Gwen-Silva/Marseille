using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatText : MonoBehaviour
{
	[SerializeField] private TMP_Text text;
	[SerializeField] private float floatHeight = 1.0f;
	[SerializeField] private float duration = 1.5f;
	[SerializeField] private float scalePop = 1.3f;
	[SerializeField] private float fadeDuration = 0.5f;

	private CanvasGroup canvasGroup;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
	}

	public void Initialize(int amount, bool isHeal)
	{
		text.text = (isHeal ? "+" : "-") + Mathf.Abs(amount).ToString();
		text.color = isHeal ? Color.green : Color.red;

		transform.localScale = Vector3.zero;

		transform.DOScale(Vector3.one * scalePop, 0.3f).SetEase(Ease.OutBack);
		transform.DOMoveY(transform.position.y + floatHeight, duration).SetEase(Ease.OutCubic);
		canvasGroup.DOFade(0, fadeDuration)
		.SetEase(Ease.InOutQuad)
		.SetDelay(duration - fadeDuration)
		.OnComplete(() =>
		{
			Destroy(gameObject);
		});
	}

	public void InitializeCustom(string message, Color color, float fontSize = 50f)
	{
		text.text = message;
		text.color = color;
		text.fontSize = fontSize;

		transform.localScale = Vector3.zero;

		transform.DOScale(Vector3.one * scalePop, 0.3f).SetEase(Ease.OutBack);
		transform.DOMoveY(transform.position.y + floatHeight, duration).SetEase(Ease.OutCubic);
		canvasGroup.DOFade(0, fadeDuration)
			.SetEase(Ease.InOutQuad)
			.SetDelay(duration - fadeDuration)
			.OnComplete(() =>
			{
				Destroy(gameObject);
			});
	}
}
