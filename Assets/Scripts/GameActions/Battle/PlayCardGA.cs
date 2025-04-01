/// <summary>
/// Represents a GameAction for playing a card to a specific slot on the board.
/// </summary>
public class PlayCardGA : GameAction
{
	#region Public Fields
	public CardDisplay card;
	public CardDropZone targetSlot;
	public bool isValueSlot;
	#endregion

	#region Constructor
	public PlayCardGA() { }
	#endregion
}
