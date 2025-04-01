using UnityEngine;
using static ExtensionMethods;

public class ResolveRoundReactions : MonoBehaviour
{
	[SerializeField] private TurnSystem turnSystem;
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;

	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<ResolveRoundGA>(GriefNullifyBeforeResolve, ReactionTiming.PRE);
		ActionSystem.SubscribeReaction<ResolveRoundGA>(ResolveRoundReaction, ReactionTiming.POST);
	}

	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ResolveRoundGA>(GriefNullifyBeforeResolve, ReactionTiming.PRE);
		ActionSystem.UnsubscribeReaction<ResolveRoundGA>(ResolveRoundReaction, ReactionTiming.POST);
	}

	private void ResolveRoundReaction(ResolveRoundGA ga)
	{
		var playerCardComponent = turnSystem.PlayerValueSlot.GetComponentInChildren<Card>();
		var opponentCardComponent = turnSystem.OpponentValueSlot.GetComponentInChildren<Card>();
		var playerEffectCard = turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>();
		var opponentEffectCard = turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>();

		bool bothAreGriefAndNullifiable =
			playerEffectCard != null &&
			opponentEffectCard != null &&
			playerEffectCard.cardData.cardEffect == CardEffect.Grief &&
			opponentEffectCard.cardData.cardEffect == CardEffect.Grief &&
			CardEffectUtils.GetTier(playerEffectCard.cardData.cardValue) <= 3 &&
			CardEffectUtils.GetTier(opponentEffectCard.cardData.cardValue) <= 3;

		var playerCard = playerCardComponent?.cardVisual?.GetComponent<CardDisplay>();
		var opponentCard = opponentCardComponent?.cardVisual?.GetComponent<CardDisplay>();

		int playerValue = GetCardValue(playerCard);
		int opponentValue = GetCardValue(opponentCard);

		bool isPlayerAttacking = turnSystem.IsPlayerStarting;

		if (isPlayerAttacking)
		{
			if (!bothAreGriefAndNullifiable || playerEffectCard.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(playerEffectCard);

			ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Medium));

			if (!bothAreGriefAndNullifiable || opponentEffectCard.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(opponentEffectCard);
		}
		else
		{
			if (!bothAreGriefAndNullifiable || playerEffectCard.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(playerEffectCard);

			ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Medium));

			if (!bothAreGriefAndNullifiable || opponentEffectCard.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(opponentEffectCard);
		}

		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Medium));

		CombatResult result;
		if (playerValue > opponentValue)
			result = isPlayerAttacking ? CombatResult.AttackSuccess : CombatResult.AttackBlocked;
		else if (playerValue < opponentValue)
			result = isPlayerAttacking ? CombatResult.AttackBlocked : CombatResult.AttackSuccess;
		else
			result = CombatResult.Tie;

		Card attacker = isPlayerAttacking ? playerCardComponent : opponentCardComponent;
		Card defender = isPlayerAttacking ? opponentCardComponent : playerCardComponent;

		ActionSystem.Instance.AddReaction(new CardCombatAnimationGA(attacker, defender, result));
		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Medium));

		int difference = isPlayerAttacking
			? playerValue - opponentValue
			: opponentValue - playerValue;

		if (difference > 0)
		{
			if (isPlayerAttacking)
			{
				ActionSystem.Instance.AddReaction(new DealDamageGA(playerCard, opponentCard, opponentHealth, difference));
			}
			else
			{
				ActionSystem.Instance.AddReaction(new DealDamageGA(opponentCard, playerCard, playerHealth, difference));
			}
			ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Short));
		}

		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Short));
		ActionSystem.Instance.AddReaction(new ClearBoardGA());
		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Short));
		ActionSystem.Instance.AddReaction(new ToggleTurnOwnerGA());
		ActionSystem.Instance.AddReaction(new WaitGA(DelayType.Short));
		ActionSystem.Instance.AddReaction(new StartGameGA());
	}

	private void TriggerEffect(Card effectCard)
	{
		if (effectCard == null || effectCard.cardData.cardEffect == CardEffect.None)
			return;

		switch (effectCard.cardData.cardEffect)
		{
			case CardEffect.Love:
				ActionSystem.Instance.AddReaction(new LoveGA(effectCard));
				break;
			case CardEffect.Grief:
				ActionSystem.Instance.AddReaction(new GriefGA(effectCard));
				break;
		}
	}

	private void GriefNullifyBeforeResolve(ResolveRoundGA ga)
	{
		var playerEffectCard = turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>();
		var opponentEffectCard = turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>();

		if (turnSystem.IsPlayerStarting)
		{
			if (playerEffectCard != null && playerEffectCard.cardData.cardEffect == CardEffect.Grief)
			{
				int tier = CardEffectUtils.GetTier(playerEffectCard.cardData.cardValue);
				if (tier >= 1 && tier <= 3)
					ActionSystem.Instance.AddReaction(new GriefNullifyEffectGA(tier, targetIsPlayer: false));
			}
			if (opponentEffectCard != null && opponentEffectCard.cardData.cardEffect == CardEffect.Grief)
			{
				int tier = CardEffectUtils.GetTier(opponentEffectCard.cardData.cardValue);
				if (tier >= 1 && tier <= 3)
					ActionSystem.Instance.AddReaction(new GriefNullifyEffectGA(tier, targetIsPlayer: true));
			}
		}
		else
		{
			if (opponentEffectCard != null && opponentEffectCard.cardData.cardEffect == CardEffect.Grief)
			{
				int tier = CardEffectUtils.GetTier(opponentEffectCard.cardData.cardValue);
				if (tier >= 1 && tier <= 3)
					ActionSystem.Instance.AddReaction(new GriefNullifyEffectGA(tier, targetIsPlayer: true));
			}
			if (playerEffectCard != null && playerEffectCard.cardData.cardEffect == CardEffect.Grief)
			{
				int tier = CardEffectUtils.GetTier(playerEffectCard.cardData.cardValue);
				if (tier >= 1 && tier <= 3)
					ActionSystem.Instance.AddReaction(new GriefNullifyEffectGA(tier, targetIsPlayer: false));
			}
		}
	}

	private int GetCardValue(CardDisplay display)
	{
		if (display == null || string.IsNullOrEmpty(display.cardTopValue.text))
			return 0;

		int.TryParse(display.cardTopValue.text, out int value);
		return value;
	}
}
