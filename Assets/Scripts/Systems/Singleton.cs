using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this as T;
	}

#if UNITY_EDITOR
	protected virtual void OnApplicationQuit()
	{
		Instance = null;
		Destroy(gameObject);
	}
#endif

	// Corrigido: nome do tipo interno alterado para TPersistent
	public abstract class PersistentSingleton<TPersistent> : Singleton<TPersistent> where TPersistent : MonoBehaviour
	{
		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}
}
