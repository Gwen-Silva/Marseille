using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HealthSystem : MonoBehaviour
{
	[SerializeField] private GameObject floatTextPrefab;
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

	public void ShowShieldText(HealthDisplay target)
	{
		Transform effectPoint = target.isPlayerHealth ? playerEffectPoint : opponentEffectPoint;

		if (floatTextPrefab != null && effectPoint != null)
		{
			GameObject obj = Instantiate(floatTextPrefab, effectPoint.position, Quaternion.identity, effectPoint);
			var floatText = obj.GetComponent<FloatText>();

			if (floatText != null)
				floatText.InitializeCustom("Protegido", new Color32(0xA7, 0x92, 0xB5, 255));
		}
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

		Transform effectPoint = ga.Target.isPlayerHealth ? playerEffectPoint : opponentEffectPoint;
		if (floatTextPrefab != null && effectPoint != null)
		{
			GameObject obj = Instantiate(floatTextPrefab, effectPoint.position, Quaternion.identity, effectPoint);
			var floatText = obj.GetComponent<FloatText>();
			floatText.Initialize(ga.Amount, false);
		}

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

		if (floatTextPrefab != null && effectPoint != null)
		{
			GameObject obj = Instantiate(floatTextPrefab, effectPoint.position, Quaternion.identity, effectPoint);
			var floatText = obj.GetComponent<FloatText>();
			floatText.Initialize(ga.Amount, true);
		}

		yield return ga.Target.AddHealth(ga.Amount);
	}
}
