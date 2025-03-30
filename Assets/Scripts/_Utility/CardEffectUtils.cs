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
}
