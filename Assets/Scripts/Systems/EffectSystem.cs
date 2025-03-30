using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
	[SerializeField] private HealthDisplay playerHealth;
	[SerializeField] private HealthDisplay opponentHealth;

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<LoveGA>(LovePerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<LoveGA>();
	}

	private IEnumerator LovePerformer(LoveGA ga)
	{
		if (ga.Card == null)
			yield break;

		int value = ga.Card.cardData.cardValue;
		bool isPlayer = ga.Card.isPlayerCard;
		int amount = 0;

		if (value >= 1 && value <= 3) amount = 1;
		else if (value >= 4 && value <= 6) amount = 2;
		else if (value >= 7 && value <= 9) amount = 3;
		else if (value == 10) amount = 5;

		if (amount > 0)
		{
			HealthDisplay target = isPlayer ? playerHealth : opponentHealth;

			var healGA = new HealHealthGA(target, amount);
			ActionSystem.Instance.AddReaction(healGA);
		}

		yield return null;
	}
}
