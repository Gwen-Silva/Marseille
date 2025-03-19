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

		if (GUILayout.Button("Draw Next Card")) {
			HandManager handManager = (HandManager)FindAnyObjectByType(typeof(HandManager));

			if (handManager != null) {
				deckManager.DrawCard(handManager);
				EditorUtility.SetDirty(deckManager);
			}
		}

		DrawDefaultInspector();
	}
}
#endif
