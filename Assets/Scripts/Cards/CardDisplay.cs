using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
	[Header("Card Data")]
	public CardData cardData;

	[Header("Visual References")]
	public Image cardImage;
	public TMP_Text cardTopValue;
	public TMP_Text cardBottomValue;

	[Header("Effect Sprites")]
	public Sprite loveSprite;
	public Sprite doubtSprite;
	public Sprite griefSprite;
	public Sprite guiltySprite;

	[Header("Neutral Sprite")]
	public Sprite valueOnlySprite;

	[HideInInspector] public Card OwnerCard;

	public void SetupDisplay(CardData data)
	{
		cardData = data;
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
			default:
				cardImage.sprite = valueOnlySprite;
				break;
		}
	}

	public void ChangeToValueSprite()
	{
		cardImage.sprite = valueOnlySprite;

		cardTopValue.gameObject.SetActive(true);
		cardTopValue.rectTransform.anchoredPosition = Vector2.zero;
		cardTopValue.fontSize = 90;

		cardBottomValue.gameObject.SetActive(false);
	}
}
