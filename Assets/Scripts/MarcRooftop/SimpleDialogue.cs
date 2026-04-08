using UnityEngine;
using TMPro;

public class SimpleDialogue : MonoBehaviour
{
	public GameObject vnUI;           // whole UI container
	public GameObject mainTextObject; // textbox panel
	public TMP_Text nameText;         // character name
	public TMP_Text dialogueText;     // SpeakText

	public void ShowDialogue(string speaker, string line)
	{
		vnUI.SetActive(true);
		mainTextObject.SetActive(true);

		nameText.text = speaker;

		// 🔥 SAME SYSTEM AS YOUR VN
		TextCreator.fullText = line;
		TextCreator.charCount = 0;
		TextCreator.runTextPrint = true;
	}
}