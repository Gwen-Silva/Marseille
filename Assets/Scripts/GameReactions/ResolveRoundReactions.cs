using UnityEngine;

public class ResolveRoundReactions : MonoBehaviour
{
	[SerializeField] private TurnSystem turnSystem;
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<ResolveRoundGA>(ResolveRoundReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ResolveRoundGA>(ResolveRoundReaction, ReactionTiming.POST);
	}

	private void ResolveRoundReaction(ResolveRoundGA ga)
	{
		Card playerCardComponent = turnSystem.PlayerValueSlot.GetComponentInChildren<Card>();
		Card opponentCardComponent = turnSystem.OpponentValueSlot.GetComponentInChildren<Card>();

		CardDisplay playerCard = playerCardComponent?.cardVisual?.GetComponent<CardDisplay>();
		CardDisplay opponentCard = opponentCardComponent?.cardVisual?.GetComponent<CardDisplay>();

		int playerValue = GetCardValue(playerCard);
		int opponentValue = GetCardValue(opponentCard);

		bool isPlayerAttacking = turnSystem.IsPlayerStarting;

		int difference = isPlayerAttacking
			? playerValue - opponentValue
			: opponentValue - playerValue;

		if (difference > 0)
		{
			if (isPlayerAttacking)
			{
				Debug.Log($"[ResolveReaction] Jogador causou {difference} de dano no oponente");
				ActionSystem.Instance.AddReaction(new DealDamageGA(playerCard, opponentCard, opponentHealth, difference));
			}
			else
			{
				Debug.Log($"[ResolveReaction] Oponente causou {difference} de dano no jogador");
				ActionSystem.Instance.AddReaction(new DealDamageGA(opponentCard, playerCard, playerHealth, difference));
			}
		}
		else if (difference < 0)
		{
			Debug.Log("[ResolveReaction] Nenhum dano aplicado (Defesa Bem Sucedida)");
		}
		else
		{
			Debug.Log("[ResolveReaction] Nenhum dano aplicado (Empate)");
		}

		ActionSystem.Instance.AddReaction(new ClearBoardGA());
		ActionSystem.Instance.AddReaction(new ToggleTurnOwnerGA());
		ActionSystem.Instance.AddReaction(new StartGameGA());
	}

	private int GetCardValue(CardDisplay display)
	{
		if (display == null || string.IsNullOrEmpty(display.cardTopValue.text))
			return 0;

		int.TryParse(display.cardTopValue.text, out int value);
		return value;
	}
}
