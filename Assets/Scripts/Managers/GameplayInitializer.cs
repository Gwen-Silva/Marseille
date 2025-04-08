using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
	private bool initialized = false;

	private void Start()
	{
		if (!initialized && ActionSystem.Instance != null)
		{
			Debug.Log("[GameplayInitializer] Inicializando gameplay com InitializeGameplayGA...");
			initialized = true;
			ActionSystem.Instance.Perform(new InitializeGameplayGA());
		}
	}
}
