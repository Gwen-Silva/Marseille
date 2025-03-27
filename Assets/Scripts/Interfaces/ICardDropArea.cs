using UnityEngine;

public interface ICardDropArea
{
	void OnCardDropped(Card card);
	void ToggleHighlight(bool state);
}
