using UnityEngine;

/// <summary>
/// Generic singleton base class for MonoBehaviours.
/// Ensures only one instance exists at runtime.
/// </summary>
/// <typeparam name="T">The type to be used as a singleton.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	#region Public Static Properties

	/// <summary>
	/// The current instance of the singleton.
	/// </summary>
	public static T Instance { get; private set; }

	#endregion

	#region Unity Lifecycle

	/// <summary>
	/// Initializes the singleton instance or destroys duplicates.
	/// </summary>
	protected virtual void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this as T;
		DontDestroyOnLoad(gameObject);
	}

#if UNITY_EDITOR
	/// <summary>
	/// Resets the instance on application quit (Editor only).
	/// </summary>
	protected virtual void OnApplicationQuit()
	{
		Instance = null;
		Destroy(gameObject);
	}
#endif

	#endregion

	#region Nested Types

	/// <summary>
	/// A persistent singleton that survives scene loads.
	/// </summary>
	/// <typeparam name="TPersistent">Type of persistent singleton.</typeparam>
	public abstract class PersistentSingleton<TPersistent> : Singleton<TPersistent> where TPersistent : MonoBehaviour
	{
		/// <summary>
		/// Prevents the object from being destroyed on scene load.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}

	#endregion
}
