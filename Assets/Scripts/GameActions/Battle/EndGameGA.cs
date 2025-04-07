/// <summary>
/// Represents the end of the game, declaring either a victory or defeat for the player.
/// </summary>
public class EndGameGA : GameAction
{
	#region Public Fields

	public bool playerWon;

	#endregion

	#region Constructor

	public EndGameGA(bool playerWon)
	{
		this.playerWon = playerWon;
	}

	#endregion
}
