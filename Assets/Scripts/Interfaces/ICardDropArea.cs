/// <summary>
/// Represents a drop area where cards can be placed.
/// </summary>
public interface ICardDropArea
{
	void OnCardDropped(Card card);
	void ToggleHighlight(bool state);
}
