using UnityEngine;

[CreateAssetMenu(menuName = "Card/Curve Parameters")]
public class CurveParameters : ScriptableObject
{
	[Header("Vertical Positioning Curve")]
	public AnimationCurve positioning = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float positioningInfluence = 50f;

	[Header("Card Rotation Curve")]
	public AnimationCurve rotation = AnimationCurve.Linear(0, -1, 1, 1);
	public float rotationInfluence = 15f;
}
