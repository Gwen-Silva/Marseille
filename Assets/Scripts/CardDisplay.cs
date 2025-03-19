using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AxolotlProductions;

public class CardDisplay : MonoBehaviour {
	public Card cardData;
	public Image cardImage;
	public TMP_Text cardTopValue;
	public TMP_Text cardBottomValue;

	public Sprite loveSprite;
	public Sprite doubtSprite;
	public Sprite griefSprite;
	public Sprite guiltySprite;

	public Sprite valueOnlySprite;

	void Start() {
		UpdateCardDisplay();
	}

	public void UpdateCardDisplay() {
		cardTopValue.text = cardData.cardValue.ToString();
		cardBottomValue.text = cardData.cardValue.ToString();

		switch (cardData.cardEffect) {
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

	public void ChangeToValueSprite() {
		cardImage.sprite = valueOnlySprite;
		cardTopValue.gameObject.SetActive(true);
		cardBottomValue.gameObject.SetActive(true);
	}

}
