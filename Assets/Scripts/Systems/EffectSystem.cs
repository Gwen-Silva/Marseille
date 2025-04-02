using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual and gameplay effects triggered by card actions such as healing or grief mechanics.
/// </summary>
public class EffectSystem : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;
	[SerializeField] private TurnSystem turnSystem;

	#endregion

	#region Unity Events

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<LoveGA>(LovePerformer);
		ActionSystem.AttachPerformer<GriefGA>(GriefPerformer);
		ActionSystem.AttachPerformer<GriefApplyShieldGA>(GriefApplyShieldPerformer);
		ActionSystem.AttachPerformer<GriefNullifyEffectGA>(GriefNullifyEffectPerformer);
		ActionSystem.AttachPerformer<GuiltGA>(GuiltPerformer);
		ActionSystem.AttachPerformer<GuiltApplyDebuffGA>(GuiltApplyDebuffPerformer);
		ActionSystem.AttachPerformer<DoubtGA>(DoubtPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<LoveGA>();
		ActionSystem.DetachPerformer<GriefGA>();
		ActionSystem.DetachPerformer<GriefApplyShieldGA>();
		ActionSystem.DetachPerformer<GriefNullifyEffectGA>();
		ActionSystem.DetachPerformer<GuiltGA>();
		ActionSystem.DetachPerformer<GuiltApplyDebuffGA>();
		ActionSystem.DetachPerformer<DoubtGA>();
	}

	#endregion

	#region Performers

	/// <summary>
	/// Heals the appropriate target based on the Love card value.
	/// </summary>
	private IEnumerator LovePerformer(LoveGA ga)
	{
		if (ga.Card == null)
			yield break;

		int value = ga.Card.cardData.cardValue;
		bool isPlayer = ga.Card.isPlayerCard;
		int healAmount = 0;

		switch (CardEffectUtils.GetTier(value))
		{
			case 1: healAmount = 1; break;
			case 2: healAmount = 2; break;
			case 3: healAmount = 3; break;
			case 4: healAmount = 5; break;
		}

		if (healAmount > 0)
		{
			HealthDisplay target = isPlayer ? playerHealth : opponentHealth;

			if (target.CurrentHealth < target.MaxHealth)
			{
				Color loveColor = CardEffectUtils.GetEffectColor(CardEffect.Love);
				ga.Card.cardVisual.PulseEffect(loveColor);

				ActionSystem.Instance.AddReaction(new HealHealthGA(target, healAmount));
			}
		}

		yield return null;
	}

	/// <summary>
	/// Applies Grief behavior based on card tier. Tier 4 gives a shield.
	/// </summary>
	private IEnumerator GriefPerformer(GriefGA ga)
	{
		if (ga.Card == null)
			yield break;

		int value = ga.Card.cardData.cardValue;
		int tier = CardEffectUtils.GetTier(value);

		if (tier == 4)
		{
			ActionSystem.Instance.AddReaction(new GriefApplyShieldGA(ga.Card));
		}

		yield return null;
	}

	/// <summary>
	/// Applies a Grief shield to the appropriate health display.
	/// </summary>
	private IEnumerator GriefApplyShieldPerformer(GriefApplyShieldGA ga)
	{
		if (ga.Card == null)
			yield break;

		bool isPlayer = ga.Card.isPlayerCard;
		HealthDisplay target = isPlayer ? playerHealth : opponentHealth;

		target.HasGriefShield = true;
		Object.FindFirstObjectByType<HealthSystem>()?.ShowShieldText(target);

		Color griefColor = CardEffectUtils.GetEffectColor(CardEffect.Grief);
		ga.Card.cardVisual.PulseEffect(griefColor);

		if (target.GriefShieldOnEffect != null && target.EffectSpawnPoint != null)
		{
			GameObject fx = Instantiate(
				target.GriefShieldOnEffect,
				target.EffectSpawnPoint.position,
				Quaternion.identity,
				target.transform
			);
			target.ActiveShieldIcon = fx;
		}

		yield return null;
	}

	/// <summary>
	/// Nullifies an opponent’s effect if their card tier is less than or equal to this card's tier.
	/// </summary>
	private IEnumerator GriefNullifyEffectPerformer(GriefNullifyEffectGA ga)
	{
		bool targetIsPlayer = ga.TargetIsPlayer;
		int tier = ga.Tier;

		Card targetCard = targetIsPlayer
			? turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>()
			: turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>();

		Card griefCard = targetIsPlayer
			? turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>()
			: turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>();

		if (targetCard == null || griefCard == null)
			yield break;

		int value = targetCard.cardData.cardValue;
		int targetTier = CardEffectUtils.GetTier(value);

		if (targetTier <= tier)
		{
			Color griefColor = CardEffectUtils.GetEffectColor(CardEffect.Grief);
			griefCard.cardVisual.PulseEffect(griefColor);

			targetCard.cardVisual.PulseNegativeEffect();
			targetCard.cardVisual.GetComponent<CardDisplay>().ChangeToValueSprite();
			targetCard.cardData.cardEffect = CardEffect.None;
		}

		yield return null;
	}

	private IEnumerator GuiltPerformer(GuiltGA ga)
	{
		if (ga.Card == null)
			yield break;

		int value = ga.Card.cardData.cardValue;
		int tier = CardEffectUtils.GetTier(value);
		bool isPlayer = ga.Card.isPlayerCard;

		Color guiltColor = CardEffectUtils.GetEffectColor(CardEffect.Guilt);

		yield return null;
	}

	private IEnumerator GuiltApplyDebuffPerformer(GuiltApplyDebuffGA ga)
	{
		Card targetCard = ga.TargetIsPlayer
			? turnSystem.PlayerValueSlot.GetComponentInChildren<Card>()
			: turnSystem.OpponentValueSlot.GetComponentInChildren<Card>();

		Card guiltCard = ga.TargetIsPlayer
			? turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>()
			: turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>();

		if (targetCard == null || guiltCard == null)
			yield break;

		if (guiltCard.cardData.cardEffect != CardEffect.Guilt)
			yield break;

		Color guiltColor = CardEffectUtils.GetEffectColor(CardEffect.Guilt);
		guiltCard.cardVisual.PulseEffect(guiltColor);

		int reduction = ga.Tier switch
		{
			1 => 1,
			2 => 2,
			3 => 3,
			4 => 5,
			_ => 0
		};

		targetCard.cardData.cardValue = Mathf.Max(0, targetCard.cardData.cardValue - reduction);

		targetCard.cardVisual.PulseNegativeEffect();

		CardDisplay display = targetCard.cardVisual.GetComponent<CardDisplay>();
		if (display != null)
		{
			string newValue = targetCard.cardData.cardValue.ToString();
			display.cardTopValue.text = newValue;
			display.cardBottomValue.text = newValue;
		}

		yield return null;
	}

	private IEnumerator DoubtPerformer(DoubtGA ga)
	{
		if (ga.Card == null)
			yield break;

		int value = ga.Card.cardData.cardValue;
		int tier = CardEffectUtils.GetTier(value);
		bool isPlayer = ga.Card.isPlayerCard;

		Color doubtColor = CardEffectUtils.GetEffectColor(CardEffect.Doubt);
		ga.Card.cardVisual.PulseEffect(doubtColor);

		if (tier == 4)
		{
			ActionSystem.Instance.AddReaction(new DoubtSwapCardsGA(1, isPlayer, true));
		}
		else
		{
			ActionSystem.Instance.AddReaction(new DoubtSwapCardsGA(tier, isPlayer));
		}

		yield return null;
	}

	#endregion
}
