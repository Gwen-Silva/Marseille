using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AxolotlProductions {
	[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
	public class Card : ScriptableObject {
		public int cardValue;
		public List<CardEffect> cardEffect;
	}

	public enum CardEffect {
		Love,
		Guilty,
		Hope,
		Grief
	}
}
