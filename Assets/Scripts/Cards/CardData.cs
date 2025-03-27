using UnityEngine;

public enum CardEffect
{
	Love,
	Guilty,
	Doubt,
	Grief
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
	public int cardValue;
	public CardEffect cardEffect;
}
