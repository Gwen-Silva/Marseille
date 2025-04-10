using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderManager : MonoBehaviour
{
	public static SceneLoaderManager Instance { get; private set; }

	[Header("Fade")]
	[SerializeField] private CanvasGroup fadeCanvasGroup;
	[SerializeField] private float fadeDuration = 0.5f;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void LoadScene(string sceneName, System.Action onSceneLoaded = null)
	{
		StartCoroutine(LoadSceneRoutine(sceneName, onSceneLoaded));
	}

	private IEnumerator LoadSceneRoutine(string sceneName, System.Action onSceneLoaded)
	{
		yield return FadeOut();

		// REMOVIDO: MonoStateUtility.ResetAllMonoStates();

		yield return null;

		SceneManager.LoadScene(sceneName);
		yield return null;

		onSceneLoaded?.Invoke();

		yield return FadeIn();
	}

	private IEnumerator FadeOut()
	{
		fadeCanvasGroup.gameObject.SetActive(true);

		float timer = 0f;
		while (timer < fadeDuration)
		{
			fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		fadeCanvasGroup.alpha = 1f;
	}

	private IEnumerator FadeIn()
	{
		float timer = 0f;
		while (timer < fadeDuration)
		{
			fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		fadeCanvasGroup.alpha = 0f;

		fadeCanvasGroup.gameObject.SetActive(false);
	}
}
