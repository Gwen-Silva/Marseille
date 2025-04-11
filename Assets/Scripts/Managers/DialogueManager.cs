using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private Image backgroundImage;
	[SerializeField] private GameObject dialogueBox;
	[SerializeField] private TextMeshProUGUI speakerNameText;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private Image playerIcon;
	[SerializeField] private Image npcIcon;

	private Queue<DialogueLine> dialogueQueue = new();
	private System.Action onDialogueComplete;

	public static DialogueManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	public void StartDialogue(DialogueSequence sequence, System.Action onComplete = null)
	{
		dialogueQueue.Clear();
		foreach (var line in sequence.lines)
			dialogueQueue.Enqueue(line);

		onDialogueComplete = onComplete;
		backgroundImage.sprite = sequence.background;

		dialogueBox.SetActive(true);
		ShowNextLine();
	}

	public void ShowNextLine()
	{
		if (dialogueQueue.Count == 0)
		{
			EndDialogue();
			return;
		}

		DialogueLine current = dialogueQueue.Dequeue();

		speakerNameText.text = current.speakerName;
		dialogueText.text = current.text;

		playerIcon.gameObject.SetActive(current.isPlayerSpeaking);
		npcIcon.gameObject.SetActive(!current.isPlayerSpeaking);

		if (current.isPlayerSpeaking)
		{
			playerIcon.sprite = current.speakerIcon;
		}
		else
		{
			npcIcon.sprite = current.speakerIcon;
		}
	}

	private void Update()
	{
		if (dialogueBox.activeSelf && Input.GetMouseButtonDown(0))
		{
			ShowNextLine();
		}
	}

	private void EndDialogue()
	{
		dialogueBox.SetActive(false);
		onDialogueComplete?.Invoke();
	}
}
