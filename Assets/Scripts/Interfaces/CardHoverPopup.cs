using TMPro;
using UnityEngine;
using DG.Tweening;

public class CardHoverPopup : MonoBehaviour
{
	[SerializeField] private TMP_Text cardValueText;
	[SerializeField] private TMP_Text cardEffectText;
	[SerializeField] private TMP_Text cardTitleDescriptionText;
	[SerializeField] private TMP_Text cardDescriptionText;
	[SerializeField] private Canvas parentCanvas;

	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		Hide();
	}

	public void Show(CardData data)
	{
		cardValueText.text = $"Valor: {data.cardValue}";
		cardEffectText.text = $"Emoção: {data.cardEffect}";
		cardTitleDescriptionText.text = "Efeito:";
		cardDescriptionText.text = $"{GetEffectDescription(data)}";

		AnimatePopupIn();
	}

	public void Hide()
	{
		if (canvasGroup != null)
			DOTween.Kill(canvasGroup);

		if (rectTransform != null)
			DOTween.Kill(rectTransform);

		canvasGroup.alpha = 0;
	}

	private void OnDestroy()
	{
		DOTween.Kill(rectTransform);
		DOTween.Kill(canvasGroup);
	}

	private void AnimatePopupIn()
	{
		if (rectTransform != null)
		{
			rectTransform.localScale = Vector3.zero;
			rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
		}

		if (canvasGroup != null)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.DOFade(1f, 0.2f);
		}
	}

	private string GetEffectDescription(CardData data)
	{
		int tier = CardEffectUtils.GetTier(data.cardValue);
		return CardEffectUtils.GetEffectDescription(data.cardEffect, tier);
	}
}
