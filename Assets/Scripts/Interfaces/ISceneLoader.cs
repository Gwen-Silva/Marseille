public interface ISceneLoader
{
	void LoadScene(string sceneName, System.Action onSceneLoaded = null);
}
