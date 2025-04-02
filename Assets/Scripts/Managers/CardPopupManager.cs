using UnityEngine;

public class CardPopupManager : MonoBehaviour
{
	public static CardPopupManager Instance;

	[SerializeField] private CardHoverPopup popupPrefab;
	private CardHoverPopup popupInstance;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		popupInstance = Instantiate(popupPrefab, transform);
		popupInstance.gameObject.SetActive(false);
	}

	public void ShowPopup(CardData data, Vector3 worldPosition)
	{
		popupInstance.transform.position = worldPosition;
		popupInstance.Show(data);
		popupInstance.gameObject.SetActive(true);
	}

	public void HidePopup()
	{
		popupInstance.Hide();
		popupInstance.gameObject.SetActive(false);
	}
}
