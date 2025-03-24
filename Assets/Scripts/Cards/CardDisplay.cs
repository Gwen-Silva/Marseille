using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
	[Header("Dados")]
	public CardData cardData;

	[Header("Referências visuais")]
	public Image cardImage;
	public TMP_Text cardTopValue;
	public TMP_Text cardBottomValue;

	[Header("Sprites de efeitos")]
	public Sprite loveSprite;
	public Sprite doubtSprite;
	public Sprite griefSprite;
	public Sprite guiltySprite;

	[Header("Sprite neutro")]
	public Sprite valueOnlySprite;

	private void Start()
	{
		UpdateCardDisplay();
	}

	public void UpdateCardDisplay()
	{
		if (cardData == null) return;

		cardTopValue.text = cardData.cardValue.ToString();
		cardBottomValue.text = cardData.cardValue.ToString();

		switch (cardData.cardEffect)
		{
			case CardEffect.Love:
				cardImage.sprite = loveSprite;
				break;
			case CardEffect.Doubt:
				cardImage.sprite = doubtSprite;
				break;
			case CardEffect.Grief:
				cardImage.sprite = griefSprite;
				break;
			case CardEffect.Guilty:
				cardImage.sprite = guiltySprite;
				break;
		}
	}

	public void ChangeToValueSprite()
	{
		cardImage.sprite = valueOnlySprite;
		cardTopValue.gameObject.SetActive(true);
		cardBottomValue.gameObject.SetActive(false);
	}
}
