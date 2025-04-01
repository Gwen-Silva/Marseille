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

		string value = cardData.cardValue.ToString();
		cardTopValue.text = value;
		cardBottomValue.text = value;

		cardImage.sprite = GetSpriteForEffect(cardData.cardEffect);
	}

	public void ChangeToValueSprite()
	{
		cardImage.sprite = valueOnlySprite;

		cardTopValue.gameObject.SetActive(true);
		cardTopValue.rectTransform.anchoredPosition = Vector2.zero;
		cardTopValue.fontSize = 90;

		cardBottomValue.gameObject.SetActive(false);
	}

	private Sprite GetSpriteForEffect(CardEffect effect)
	{
		return effect switch
		{
			CardEffect.Love => loveSprite,
			CardEffect.Doubt => doubtSprite,
			CardEffect.Grief => griefSprite,
			CardEffect.Guilty => guiltySprite,
			_ => valueOnlySprite
		};
	}
}
