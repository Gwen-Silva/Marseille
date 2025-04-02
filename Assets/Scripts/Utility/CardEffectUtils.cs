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
	public static readonly Color GuiltyColor = new Color(0.1f, 0.2f, 0.4f); // dark blue
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
			CardEffect.Guilt => GuiltyColor,
			CardEffect.Doubt => DoubtColor,
			_ => DefaultColor
		};
	}

	public static string GetEffectDescription(CardEffect effect, int tier)
	{
		switch (effect)
		{
			case CardEffect.Love:
				return tier switch
				{
					1 => "Cura 1 de Vida",
					2 => "Cura 2 de Vida",
					3 => "Cura 3 de Vida",
					4 => "Cura 5 de Vida",
					_ => "Sem Efeito"
				};
			case CardEffect.Grief:
				return tier switch
				{
					1 => "Anula efeitos Fracos",
					2 => "Anula efeitos Médios ou menores",
					3 => "Anula efeitos Fortes ou menores",
					4 => "Recebe um Escudo que Anula o próximo Dano Letal",
					_ => "Sem Efeito"
				};
			case CardEffect.Guilt:
				return tier switch
				{
					1 => "Reduz em 1 o Valor do Argumento do Oponente",
					2 => "Reduz em 2 o Valor do Argumento do Oponente",
					3 => "Reduz em 3 o Valor do Argumento do Oponente",
					4 => "Reduz em 5 o Valor do Argumento do Oponente",
					_ => "Sem Efeito"
				};
			default: return "Sem Efeito";
		}
	}

	public static string GetEffectNameLocalized(CardEffect effect)
	{
		return effect switch
		{
			CardEffect.Love => "Amor",
			CardEffect.Grief => "Luto",
			CardEffect.Guilt => "Culpa",
			CardEffect.Doubt => "Dúvida",
			CardEffect.None => "Nenhum",
			_ => effect.ToString()
		};
	}
}
