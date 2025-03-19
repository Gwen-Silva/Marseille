using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxolotlProductions;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DeckManager))]
public class DeckManagerEditor : Editor {
	public override void OnInspectorGUI() {
		DeckManager deckManager = (DeckManager)target;

		if (GUILayout.Button("Draw Next Card (Player)")) {
			BaseHandManager playerHand = FindAnyObjectByType<HandManager>();

			if (playerHand != null) {
				deckManager.DrawCard(playerHand, true);
				EditorUtility.SetDirty(deckManager);
			}
		}

		if (GUILayout.Button("Draw Next Card (Opponent)")) {
			BaseHandManager opponentHand = FindAnyObjectByType<OpponentHandManager>();

			if (opponentHand != null) {
				deckManager.DrawCard(opponentHand, false);
				EditorUtility.SetDirty(deckManager);
			}
		}

		DrawDefaultInspector();
	}
}
#endif
