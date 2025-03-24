using UnityEngine;

namespace AxolotlProductions {
	[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
	public class Card : ScriptableObject {
		public int cardValue;
		public CardEffect cardEffect;

		public void Initialize(int value, CardEffect effect) {
			cardValue = value;
			cardEffect = effect;
		}
	}

	public enum CardEffect {
		Love,
		Guilty,
		Doubt,
		Grief,
		Anger,
		Simpathy,
	}
}
