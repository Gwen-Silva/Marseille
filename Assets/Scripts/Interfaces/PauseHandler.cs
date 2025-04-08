using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PauseHandler : MonoBehaviour
{
	[SerializeField] private GameObject systemsRoot;
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private RectTransform mainPanel;
	[SerializeField] private RectTransform titlePanel;
	[SerializeField] private RectTransform menuPanel;
	[SerializeField] private RectTransform optionsPanel;
	[SerializeField] private RectTransform resumePanel;

	private void OnEnable()
	{
		GameStateManager.OnGameStateChanged += HandleState;
	}

	private void OnDisable()
	{
		GameStateManager.OnGameStateChanged -= HandleState;
	}

	private void HandleState(GameState state)
	{
		bool shouldPause = (state == GameState.Paused);

		if (systemsRoot != null)
			systemsRoot.SetActive(!shouldPause);

		if (pausePanel != null)
		{
			if (shouldPause)
			{
				pausePanel.SetActive(true);
				ResetPanels();
				StartCoroutine(PlayPanelSequence());
			}
		}
	}

	private void ResetPanels()
	{
		mainPanel.localScale = Vector3.zero;
		titlePanel.localScale = Vector3.zero;
		menuPanel.localScale = Vector3.zero;
		optionsPanel.localScale = Vector3.zero;
		resumePanel.localScale = Vector3.zero;
	}

	private IEnumerator PlayPanelSequence()
	{
		float bounceDuration = 0.3f;
		float delayBetween = 0.1f;

		mainPanel.DOScale(1f, bounceDuration).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(bounceDuration + delayBetween);

		titlePanel.DOScale(1f, bounceDuration).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(delayBetween);

		menuPanel.DOScale(1f, bounceDuration).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(delayBetween);

		optionsPanel.DOScale(1f, bounceDuration).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(delayBetween);

		resumePanel.DOScale(1f, bounceDuration).SetEase(Ease.OutBack);
	}

	private IEnumerator HidePanelSequence()
	{
		float hideDuration = 0.2f;

		mainPanel.DOScale(0f, hideDuration).SetEase(Ease.InBack);
		titlePanel.DOScale(0f, hideDuration).SetEase(Ease.InBack);
		menuPanel.DOScale(0f, hideDuration).SetEase(Ease.InBack);
		optionsPanel.DOScale(0f, hideDuration).SetEase(Ease.InBack);
		resumePanel.DOScale(0f, hideDuration).SetEase(Ease.InBack);

		yield return new WaitForSeconds(hideDuration);
		pausePanel.SetActive(false);
	}

	public void RequestResume()
	{
		StartCoroutine(ResumeWithAnimation());
	}

	private IEnumerator ResumeWithAnimation()
	{
		yield return HidePanelSequence();
		GameStateManager.Instance.ChangeState(GameState.Playing);
	}
}