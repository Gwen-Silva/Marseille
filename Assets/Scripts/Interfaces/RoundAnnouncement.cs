using TMPro;
using UnityEngine;
using DG.Tweening;

public class RoundAnnouncement : MonoBehaviour
{
	#region Constants

	private const float SCALE_BACK_DURATION = 0.3f;
	private const string ROUND_LABEL_FORMAT = "Rodada {0}";

	#endregion

	#region Serialized Fields

	[SerializeField] private TMP_Text roundText;
	[SerializeField] private ParticleSystem particles;
	[SerializeField] private float scaleDuration = 0.75f;
	[SerializeField] private float displayTime = 0.5f;
	[SerializeField] private Vector3 targetScale = Vector3.one;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		if (roundText == null)
			roundText = GetComponentInChildren<TMP_Text>();

		transform.localScale = Vector3.zero;
		if (roundText != null)
			roundText.gameObject.SetActive(false);
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Displays the round announcement with animation and optional particle effect.
	/// </summary>
	/// <param name="roundNumber">The current round number to display.</param>
	public void ShowRound(int roundNumber)
	{
		if (roundText == null)
			roundText = GetComponentInChildren<TMP_Text>();

		if (roundText == null)
			return;

		roundText.text = string.Format(ROUND_LABEL_FORMAT, roundNumber);
		roundText.gameObject.SetActive(true);
		transform.localScale = Vector3.zero;

		particles?.Play();

		Sequence seq = DOTween.Sequence();
		seq.Append(transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack));
		seq.AppendInterval(displayTime);
		seq.Append(transform.DOScale(Vector3.zero, SCALE_BACK_DURATION).SetEase(Ease.InBack));
		seq.OnComplete(() => roundText.gameObject.SetActive(false));
	}

	#endregion
}
