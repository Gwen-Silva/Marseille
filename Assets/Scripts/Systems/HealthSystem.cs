using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HealthSystem : MonoBehaviour
{
	[SerializeField] private GameObject healEffectPrefab;
	[SerializeField] private Transform playerEffectPoint;
	[SerializeField] private Transform opponentEffectPoint;

	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
		ActionSystem.AttachPerformer<HealHealthGA>(HealHealthPerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<DealDamageGA>();
		ActionSystem.DetachPerformer<HealHealthGA>();
	}

	private IEnumerator DealDamagePerformer(DealDamageGA ga)
	{
		if (ga.AttackerCard == null || ga.Target == null)
			yield break;

		Transform attackerTransform = ga.AttackerCard.transform;
		Vector3 originalPosition = attackerTransform.position;

		yield return attackerTransform.DOMove(ga.Target.transform.position, 0.3f)
			.SetEase(Ease.OutQuad)
			.WaitForCompletion();

		ga.Target.transform.DOShakePosition(0.5f, 0.5f, 10);

		yield return attackerTransform.DOMove(originalPosition, 0.3f)
			.SetEase(Ease.OutQuad)
			.WaitForCompletion();

		yield return ga.Target.ReduceHealth(ga.Amount);
	}
	private IEnumerator HealHealthPerformer(HealHealthGA ga)
	{
		if (ga.Target == null)
			yield break;

		Transform targetTransform = ga.Target.transform;
		Vector3 originalScale = targetTransform.localScale;

		yield return targetTransform
			.DOScale(originalScale * 1.2f, 0.3f)
			.SetEase(Ease.OutBack)
			.WaitForCompletion();

		targetTransform
			.DOScale(originalScale, 0.3f)
			.SetEase(Ease.InOutQuad);

		Transform effectPoint = ga.Target.isPlayerHealth ? playerEffectPoint : opponentEffectPoint;

		if (healEffectPrefab != null && effectPoint != null)
		{
			GameObject vfx = Instantiate(healEffectPrefab, effectPoint.position, Quaternion.identity);
			Destroy(vfx, 2f);
		}

		yield return ga.Target.AddHealth(ga.Amount);
	}


}
