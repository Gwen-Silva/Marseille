using UnityEngine;

public enum CardEffect
{
	None = 0,
	Love = 1,
	Guilt = 2,
	Doubt = 3,
	Grief = 4,
	Anger = 5,
	Sympathy = 6,
	Empathy = 7
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
	public int cardValue;
	public CardEffect cardEffect;
}
