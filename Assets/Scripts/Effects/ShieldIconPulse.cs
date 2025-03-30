using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShieldIconPulse : MonoBehaviour
{
	[SerializeField] private float pulseScale = 0.1f;
	[SerializeField] private float pulseDuration = 2.5f;
	[SerializeField] private float targetAlpha = 0.6f;
	[SerializeField] private float appearDuration = 0.8f;

	private float minAlpha = 0.1f;

	private RectTransform rectTransform;
	private Image image;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();
	}

	private void OnEnable()
	{
		if (rectTransform == null || image == null)
			return;

		DOTween.Kill(rectTransform);
		DOTween.Kill(image);

		rectTransform.localScale = Vector3.zero;
		image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);

		Sequence appearSequence = DOTween.Sequence();
		appearSequence.Append(rectTransform.DOScale(Vector3.one, appearDuration).SetEase(Ease.OutBack));
		appearSequence.Join(image.DOFade(targetAlpha, appearDuration));

		appearSequence.OnComplete(() =>
		{
			StartPulsing();
		});
	}

	private void OnDisable()
	{
		DOTween.Kill(rectTransform);
		DOTween.Kill(image);
	}

	private void StartPulsing()
	{
		Vector3 baseScale = Vector3.one;
		Vector3 maxScale = baseScale * (1f + pulseScale);

		image.DOFade(minAlpha, pulseDuration / 2f)
			.SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine);

		rectTransform.DOScale(maxScale, pulseDuration / 2f)
			.SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine);
	}
}
