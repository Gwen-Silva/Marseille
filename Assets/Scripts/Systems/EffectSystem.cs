using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;
	[SerializeField] private TurnSystem turnSystem;

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<LoveGA>(LovePerformer);
		ActionSystem.AttachPerformer<GriefGA>(GriefPerformer);
		ActionSystem.AttachPerformer<GriefApplyShieldGA>(GriefApplyShieldPerformer);
		ActionSystem.AttachPerformer<GriefNullifyEffectGA>(GriefNullifyEffectPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<LoveGA>();
		ActionSystem.DetachPerformer<GriefGA>();
		ActionSystem.DetachPerformer<GriefApplyShieldGA>();
		ActionSystem.DetachPerformer<GriefNullifyEffectGA>();
	}

	private IEnumerator LovePerformer(LoveGA ga)
	{
		if (ga.Card == null)
			yield break;

		int value = ga.Card.cardData.cardValue;
		bool isPlayer = ga.Card.isPlayerCard;
		int amount = 0;

		switch (CardEffectUtils.GetTier(value))
		{
			case 1: amount = 1; break;
			case 2: amount = 2; break;
			case 3: amount = 3; break;
			case 4: amount = 5; break;
		}

		if (amount > 0)
		{
			HealthDisplay target = isPlayer ? playerHealth : opponentHealth;
			ActionSystem.Instance.AddReaction(new HealHealthGA(target, amount));
		}

		yield return null;
	}

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

	private IEnumerator GriefApplyShieldPerformer(GriefApplyShieldGA ga)
	{
		if (ga.Card == null)
			yield break;

		bool isPlayer = ga.Card.isPlayerCard;
		HealthDisplay target = isPlayer ? playerHealth : opponentHealth;

		target.HasGriefShield = true;

		if (target.GriefShieldOnEffect != null && target.EffectSpawnPoint != null)
		{
			GameObject fx = Instantiate(target.GriefShieldOnEffect, target.EffectSpawnPoint.position, Quaternion.identity, target.transform);
			target.ActiveShieldIcon = fx;
		}

		yield return null;
	}

	private IEnumerator GriefNullifyEffectPerformer(GriefNullifyEffectGA ga)
	{
		bool targetIsPlayer = ga.TargetIsPlayer;
		int tier = ga.Tier;

		Card card = targetIsPlayer
			? turnSystem.PlayerEffectSlot.GetComponentInChildren<Card>()
			: turnSystem.OpponentEffectSlot.GetComponentInChildren<Card>();

		if (card == null)
			yield break;

		int value = card.cardData.cardValue;
		int targetTier = CardEffectUtils.GetTier(value);

		if (targetTier <= tier)
		{
			card.cardData.cardEffect = CardEffect.None;
		}

		yield return null;
	}
}
