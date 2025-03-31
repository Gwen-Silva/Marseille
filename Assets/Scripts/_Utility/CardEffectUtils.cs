using UnityEngine;

public static class CardEffectUtils
{
	public static int GetTier(int value)
	{
		if (value >= 1 && value <= 3) return 1;
		if (value >= 4 && value <= 6) return 2;
		if (value >= 7 && value <= 9) return 3;
		if (value == 10) return 4;
		return 0;
	}

	public static Color GetEffectColor(CardEffect effect)
	{
		return effect switch
		{
			CardEffect.Love => new Color(1f, 0.53f, 0.76f),
			CardEffect.Grief => new Color(0.54f, 0.39f, 0.82f),
			CardEffect.Guilty => new Color(0.69f, 0.28f, 0.28f),
			CardEffect.Doubt => new Color(0.27f, 0.83f, 1f),
			_ => Color.white
		};
	}
}
