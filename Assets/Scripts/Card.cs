using UnityEngine;

namespace AxolotlProductions {
    public class Card {
        private readonly CardData cardData;
        public Card(CardData cardData) {
            this.cardData = cardData;
            Value = cardData.Value;
            Effect = cardData.Effect;

        }

        public Sprite Sprite { get => cardData.Sprite; }
        public int Value { get; set; }
        public string Effect { get; set; }

        public void PerformEffect() {
            Debug.Log(Effect + " Performed & Value of " + Value + " played");
        }
    }
}


