using UnityEngine;

public class ResolveRoundReactions : MonoBehaviour
{
	#region Constants

	private const int MAX_NULLIFIABLE_TIER = 3;

	#endregion

	#region Serialized Fields

	[SerializeField] private TurnSystem turnSystem;
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;

	#endregion

	#region Unity Events

	/// <summary>
	/// Subscribes to ResolveRoundGA reactions.
	/// </summary>
	private void OnEnable()
	{
		ActionSystem.SubscribeReaction<ResolveRoundGA>(GriefNullifyBeforeResolve, ReactionTiming.PRE);
		ActionSystem.SubscribeReaction<ResolveRoundGA>(ResolveRoundReaction, ReactionTiming.POST);
	}

	/// <summary>
	/// Unsubscribes from ResolveRoundGA reactions.
	/// </summary>
	private void OnDisable()
	{
		ActionSystem.UnsubscribeReaction<ResolveRoundGA>(GriefNullifyBeforeResolve, ReactionTiming.PRE);
		ActionSystem.UnsubscribeReaction<ResolveRoundGA>(ResolveRoundReaction, ReactionTiming.POST);
	}

	#endregion

	#region Reactions

	/// <summary>
	/// Executes the reaction to resolve the round, including card effects, combat resolution, and damage application.
	/// </summary>
	private void ResolveRoundReaction(ResolveRoundGA ga)
	{
		var playerCardComponent = turnSystem.PlayerValueSlot.GetComponentInChildren<Card>();
		var opponentCardComponent = turnSystem.OpponentValueSlot.GetComponentInChildren<Card>();
		var playerEffectCard = turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>();
		var opponentEffectCard = turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>();

		bool bothAreGriefAndNullifiable = AreBothGriefAndNullifiable(playerEffectCard, opponentEffectCard);

		var playerCard = playerCardComponent?.cardVisual?.GetComponent<CardDisplay>();
		var opponentCard = opponentCardComponent?.cardVisual?.GetComponent<CardDisplay>();

		int playerValue = GetCardValue(playerCard);
		int opponentValue = GetCardValue(opponentCard);

		bool isPlayerAttacking = turnSystem.IsPlayerStarting;

		PlayEffectsWithConditions(playerEffectCard, opponentEffectCard, isPlayerAttacking, bothAreGriefAndNullifiable);

		ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Medium));

		CombatResult result = DetermineCombatResult(playerValue, opponentValue, isPlayerAttacking);

		Card attacker = isPlayerAttacking ? playerCardComponent : opponentCardComponent;
		Card defender = isPlayerAttacking ? opponentCardComponent : playerCardComponent;

		ActionSystem.Shared?.AddReaction(new CardCombatAnimationGA(attacker, defender, result));
		ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Medium));

		int difference = isPlayerAttacking
			? playerValue - opponentValue
			: opponentValue - playerValue;

		if (result == CombatResult.AttackSuccess && difference > 0)
		{
			ApplyDamage(playerCard, opponentCard, difference, isPlayerAttacking);
			ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Short));
		}

		ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Short));
		ActionSystem.Shared?.AddReaction(new ClearBoardGA());
		ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Short));
		ActionSystem.Shared?.AddReaction(new ToggleTurnOwnerGA());
		ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Short));
		ActionSystem.Shared?.AddReaction(new StartGameGA());
	}

	/// <summary>
	/// Executes the Grief nullification reaction before the round is resolved.
	/// </summary>
	private void GriefNullifyBeforeResolve(ResolveRoundGA ga)
	{
		var playerEffectCard = turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>();
		var opponentEffectCard = turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>();

		if (turnSystem.IsPlayerStarting)
		{
			TryAddGriefNullify(playerEffectCard, false);
			TryAddGriefNullify(opponentEffectCard, true);
			TryAddGuiltDebuff(playerEffectCard, false);
			TryAddGuiltDebuff(opponentEffectCard, true);
		}
		else
		{
			TryAddGriefNullify(opponentEffectCard, true);
			TryAddGriefNullify(playerEffectCard, false);
			TryAddGuiltDebuff(playerEffectCard, false);
			TryAddGuiltDebuff(opponentEffectCard, true);
		}
	}

	#endregion

	#region Helpers

	private bool AreBothGriefAndNullifiable(Card cardA, Card cardB)
	{
		return cardA != null && cardB != null &&
			   cardA.cardData.cardEffect == CardEffect.Grief &&
			   cardB.cardData.cardEffect == CardEffect.Grief &&
			   CardEffectUtils.GetTier(cardA.cardData.cardValue) <= MAX_NULLIFIABLE_TIER &&
			   CardEffectUtils.GetTier(cardB.cardData.cardValue) <= MAX_NULLIFIABLE_TIER;
	}

	private void PlayEffectsWithConditions(Card playerEffect, Card opponentEffect, bool isPlayerFirst, bool skipEffects)
	{
		if (isPlayerFirst)
		{
			if (!skipEffects || playerEffect?.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(playerEffect);

			ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Medium));

			if (!skipEffects || opponentEffect?.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(opponentEffect);
		}
		else
		{
			if (!skipEffects || opponentEffect?.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(opponentEffect);

			ActionSystem.Shared?.AddReaction(new WaitGA(DelayType.Medium));

			if (!skipEffects || playerEffect?.cardData.cardEffect != CardEffect.Grief)
				TriggerEffect(playerEffect);
		}
	}

	private CombatResult DetermineCombatResult(int playerVal, int opponentVal, bool isPlayerAtk)
	{
		if (playerVal > opponentVal)
			return isPlayerAtk ? CombatResult.AttackSuccess : CombatResult.AttackBlocked;
		if (playerVal < opponentVal)
			return isPlayerAtk ? CombatResult.AttackBlocked : CombatResult.AttackSuccess;
		return CombatResult.Tie;
	}

	private void ApplyDamage(CardDisplay playerCard, CardDisplay opponentCard, int dmg, bool playerIsAttacking)
	{
		if (playerIsAttacking)
		{
			ActionSystem.Shared?.AddReaction(new DealDamageGA(playerCard, opponentCard, opponentHealth, dmg));
		}
		else
		{
			ActionSystem.Shared?.AddReaction(new DealDamageGA(opponentCard, playerCard, playerHealth, dmg));
		}
	}

	private void TryAddGriefNullify(Card card, bool targetIsPlayer)
	{
		if (card != null && card.cardData.cardEffect == CardEffect.Grief)
		{
			int tier = CardEffectUtils.GetTier(card.cardData.cardValue);
			if (tier >= 1 && tier <= MAX_NULLIFIABLE_TIER)
				ActionSystem.Shared?.AddReaction(new GriefNullifyEffectGA(tier, targetIsPlayer));
		}
	}

	private void TryAddGuiltDebuff(Card card, bool targetIsPlayer)
	{
		if (card != null && card.cardData.cardEffect == CardEffect.Guilt)
		{
			int tier = CardEffectUtils.GetTier(card.cardData.cardValue);
			ActionSystem.Shared?.AddReaction(new GuiltApplyDebuffGA(tier, targetIsPlayer));
		}
	}

	private void TriggerEffect(Card effectCard)
	{
		if (effectCard == null || effectCard.cardData.cardEffect == CardEffect.None)
			return;

		switch (effectCard.cardData.cardEffect)
		{
			case CardEffect.Love:
				ActionSystem.Shared?.AddReaction(new LoveGA(effectCard));
				break;
			case CardEffect.Grief:
				ActionSystem.Shared?.AddReaction(new GriefGA(effectCard));
				break;
			case CardEffect.Guilt:
				ActionSystem.Shared?.AddReaction(new GuiltGA(effectCard));
				break;
			case CardEffect.Doubt:
				ActionSystem.Shared?.AddReaction(new DoubtGA(effectCard));
				break;
		}
	}

	private int GetCardValue(CardDisplay display)
	{
		if (display == null || string.IsNullOrEmpty(display.cardTopValue.text))
			return 0;

		return display.OwnerCard.cardData.cardValue;
	}

	#endregion
}