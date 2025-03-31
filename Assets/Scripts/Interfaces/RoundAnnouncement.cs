using TMPro;
using UnityEngine;
using DG.Tweening;

public class RoundAnnouncement : MonoBehaviour
{
	[SerializeField] private TMP_Text roundText;
	[SerializeField] private ParticleSystem particles;
	[SerializeField] private float scaleDuration = 0.75f;
	[SerializeField] private float displayTime = 0.5f;
	[SerializeField] private Vector3 targetScale = Vector3.one;

	private void Awake()
	{
		if (roundText == null)
			roundText = GetComponentInChildren<TMP_Text>();

		transform.localScale = Vector3.zero;
		roundText.gameObject.SetActive(false);
	}

	public void ShowRound(int roundNumber)
	{
		if (roundText == null)
			roundText = GetComponentInChildren<TMP_Text>();

		roundText.text = $"Rodada {roundNumber}";
		roundText.gameObject.SetActive(true);
		transform.localScale = Vector3.zero;

		particles?.Play();

		Sequence seq = DOTween.Sequence();
		seq.Append(transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack));
		seq.AppendInterval(displayTime);
		seq.Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
		seq.OnComplete(() => roundText.gameObject.SetActive(false));
	}
}
