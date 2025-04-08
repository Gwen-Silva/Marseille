using UnityEngine;

public class PersistentSystemLoader : MonoBehaviour
{
	[Header("Prefabs de sistemas persistentes")]
	[Tooltip("Arraste aqui os prefabs como GameStateManager, AudioManager, etc.")]
	[SerializeField] private GameObject[] persistentPrefabs;

	private void Awake()
	{
		foreach (var prefab in persistentPrefabs)
		{
			TryLoadPersistentSystem(prefab);
		}

		DontDestroyOnLoad(gameObject); // Opcional: persistir o loader entre cenas
	}

	private void TryLoadPersistentSystem(GameObject prefab)
	{
		if (prefab == null)
		{
			Debug.LogWarning("[PersistentSystemLoader] Prefab nulo encontrado na lista.");
			return;
		}

		var type = prefab.GetComponent<MonoBehaviour>()?.GetType();
		if (type == null)
		{
			Debug.LogWarning($"[PersistentSystemLoader] Prefab {prefab.name} não possui MonoBehaviour.");
			return;
		}

		var existing = FindFirstInstanceOfType(type) as Object;
		if (existing != null)
		{
			Debug.Log($"[PersistentSystemLoader] {type.Name} já existe na cena.");
			return;
		}

		var instance = Instantiate(prefab);
		Debug.Log($"[PersistentSystemLoader] Instanciado {type.Name}.");
	}

	private Object FindFirstInstanceOfType(System.Type type)
	{
		var methods = typeof(Object).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
		foreach (var method in methods)
		{
			if (method.Name == "FindFirstObjectByType" && method.IsGenericMethod && method.GetParameters().Length == 0)
			{
				var generic = method.MakeGenericMethod(type);
				var result = generic.Invoke(null, null);
				return result as Object;
			}
		}

		Debug.LogError($"[PersistentSystemLoader] Método FindFirstObjectByType<T>() não encontrado.");
		return null;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void EnsureLoaderExists()
	{
		var existing = Object.FindFirstObjectByType<PersistentSystemLoader>();
		if (existing != null) return;

		var prefab = Resources.Load<GameObject>("PersistentSystemLoader");
		if (prefab != null)
		{
			GameObject.Instantiate(prefab);
			Debug.Log("[PersistentSystemLoader] Instanciado automaticamente via Resources.");
		}
		else
		{
			Debug.LogError("[PersistentSystemLoader] Prefab 'PersistentSystemLoader' não encontrado na pasta Resources.");
		}
	}
}
