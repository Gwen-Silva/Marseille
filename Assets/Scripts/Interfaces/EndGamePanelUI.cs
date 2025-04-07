using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGamePanelUI : MonoBehaviour
{
	[SerializeField] private Image backgroundImage;
	[SerializeField] private TextMeshProUGUI titleText;

	[SerializeField] private Sprite victorySprite;
	[SerializeField] private Sprite defeatSprite;

	[SerializeField] private string victoryText = "Vitória!";
	[SerializeField] private string defeatText = "Derrota...";

	public void Setup(bool playerWon)
	{
		backgroundImage.sprite = playerWon ? victorySprite : defeatSprite;
		titleText.text = playerWon ? victoryText : defeatText;
	}
}
