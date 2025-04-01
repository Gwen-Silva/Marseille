using UnityEngine;

public static class CardEffectUtils
{
	#region Constants - Tiers
	private const int Tier1Min = 1;
	private const int Tier1Max = 3;
	private const int Tier2Min = 4;
	private const int Tier2Max = 6;
	private const int Tier3Min = 7;
	private const int Tier3Max = 9;
	private const int Tier4Exact = 10;
	#endregion

	#region Constants - Colors
	public static readonly Color LoveColor = new Color(1f, 0.53f, 0.76f); // pink
	public static readonly Color GriefColor = new Color(0.54f, 0.39f, 0.82f); // lavender
	public static readonly Color GuiltyColor = new Color(0.69f, 0.28f, 0.28f); // dark red
	public static readonly Color DoubtColor = new Color(0.27f, 0.83f, 1f); // cyan
	public static readonly Color DefaultColor = Color.white;
	#endregion

	/// <summary>
	/// Returns the tier (1 to 4) based on card value.
	/// </summary>
	public static int GetTier(int value)
	{
		if (value >= Tier1Min && value <= Tier1Max) return 1;
		if (value >= Tier2Min && value <= Tier2Max) return 2;
		if (value >= Tier3Min && value <= Tier3Max) return 3;
		if (value == Tier4Exact) return 4;
		return 0;
	}

	/// <summary>
	/// Returns a color corresponding to the effect type.
	/// </summary>
	public static Color GetEffectColor(CardEffect effect)
	{
		return effect switch
		{
			CardEffect.Love => LoveColor,
			CardEffect.Grief => GriefColor,
			CardEffect.Guilty => GuiltyColor,
			CardEffect.Doubt => DoubtColor,
			_ => DefaultColor
		};
	}
}
