using UnityEngine;

public class VisualCardsHandler : MonoBehaviour
{
	public static VisualCardsHandler Instance;

	private void Awake()
	{
		Instance = this;
	}

	void Start() { }

	void Update() { }
}
