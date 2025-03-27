using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HealthSystem : MonoBehaviour
{
	private void OnEnable()
	{
		ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
	}

	private void OnDisable()
	{
		ActionSystem.DetachPerformer<DealDamageGA>();
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
}
