using UnityEngine;
using DG.Tweening;

public class EndGameReactions : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private EndGamePanelUI endGamePanel;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<EndGameGA>(OnEndGame, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<EndGameGA>(OnEndGame, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	private void OnEndGame(EndGameGA ga)
	{
		endGamePanel.gameObject.SetActive(true);
		endGamePanel.transform.localScale = Vector3.one * 0.8f;

		CanvasGroup cg = endGamePanel.GetComponent<CanvasGroup>();
		if (cg == null)
			cg = endGamePanel.gameObject.AddComponent<CanvasGroup>();

		cg.alpha = 0f;
		endGamePanel.Setup(ga.playerWon);

		cg.DOFade(1f, 0.4f).SetEase(Ease.OutQuad);
		endGamePanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
	}

	#endregion
}
