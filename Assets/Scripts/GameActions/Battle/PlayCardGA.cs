/// <summary>
/// Represents a GameAction for playing a card to a specific slot on the board.
/// </summary>
public class PlayCardGA : GameAction
{
	#region Public Fields

	public CardDisplay Card;
	public CardDropZone TargetSlot;
	public bool IsValueSlot;

	#endregion

	#region Constructor

	public PlayCardGA() { }

	#endregion
}
