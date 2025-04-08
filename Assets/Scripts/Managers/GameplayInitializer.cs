using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
	private bool initialized = false;

	private void Start()
	{
		if (!initialized && ActionSystem.Shared != null)
		{
			Debug.Log("[GameplayInitializer] Inicializando gameplay com InitializeGameplayGA...");
			initialized = true;
			ActionSystem.Shared?.Perform(new InitializeGameplayGA());
		}
	}
}
