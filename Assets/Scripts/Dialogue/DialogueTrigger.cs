using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private DialogueSequence sequence;

	void Start()
	{
		DialogueManager.Instance.StartDialogue(sequence, () =>
		{
			Debug.Log("Diálogo finalizado!");
		});
	}
}
