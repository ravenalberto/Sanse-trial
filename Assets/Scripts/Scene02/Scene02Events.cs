using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ChairPuzzleManager;

public class Scene02Events : MonoBehaviour
{

	public GameObject charKuh;
	public GameObject charCristel;
	public GameObject charDarlene;
	public GameObject charMarc;
	public GameObject charRaven;
	public GameObject textBox;

	[SerializeField] GameObject vnUI;


	[SerializeField] string textToSpeak;
	[SerializeField] int currentTextLength;
	[SerializeField] int textLength;
	[SerializeField] GameObject mainTextObject;
	[SerializeField] GameObject nextButton;
	[SerializeField] int eventPos = 0;
	[SerializeField] GameObject charName;
	[SerializeField] GameObject fadeOut;

	[SerializeField] Sprite cristelNeutral;
	[SerializeField] Sprite cristelFrown;

	[SerializeField] Sprite kuhNeutral;


	[SerializeField] Sprite marcNeutral;
	[SerializeField] Sprite marcSmile;

	[SerializeField] Sprite ravenNeutral;

	[SerializeField] Sprite darleneNeutral;
	[SerializeField] Sprite darleneShock;
	

	[SerializeField] GameObject fadeScreenIn;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		fadeScreenIn.SetActive(true);
		StartCoroutine(FadeIn());

		if (PuzzleState.scene2Result != "")
		{
			// 🎯 RETURNING FROM PUZZLE
			StartCoroutine(HandlePuzzleResult());
		}
		else
		{
			// 🎬 NORMAL START
			StartCoroutine(EventStarter());
		}
	}

	IEnumerator FadeIn()
	{
		CanvasGroup fadeCanvas = fadeScreenIn.GetComponent<CanvasGroup>();

		float t = 1;

		while (t > 0)
		{
			t -= Time.deltaTime;
			fadeCanvas.alpha = t;
			yield return null;
		}

		fadeScreenIn.SetActive(false);
	}
	// Update is called once per frame
	void Update()
    {
		textLength = TextCreator.charCount;
	}

	IEnumerator HandlePuzzleResult()
	{
		yield return new WaitForSeconds(1f);

		string result = PuzzleState.scene2Result;
		PuzzleState.scene2Result = "";

		vnUI.SetActive(true);
		nextButton.SetActive(true);

		if (result == "Approve")
		{
			eventPos = 20;
		}
		else if (result == "Disapprove")
		{
			eventPos = 30;
		}
		else
		{
			eventPos = 40;
		}
	}

	IEnumerator SlideCharacter(GameObject character, Vector3 start, Vector3 end, float duration)
	{
		RectTransform rect = character.GetComponent<RectTransform>();

		float time = 0;
		rect.anchoredPosition = start;

		while (time < duration)
		{
			rect.anchoredPosition = Vector3.Lerp(start, end, time / duration);
			time += Time.deltaTime;
			yield return null;
		}

		rect.anchoredPosition = end;
	}

	IEnumerator ShowText(string speaker, string line)
	{
		mainTextObject.SetActive(true);
		textBox.GetComponent<TMPro.TMP_Text>().text = line;

		// Optional: set name
		// charName.GetComponent<TMP_Text>().text = speaker;

		yield return new WaitForSeconds(2f); // replace with button later
	}

	IEnumerator EventStarter()
	{
		yield return new WaitForSeconds(1);

		fadeScreenIn.SetActive(false);

		// Darlene enters
		charDarlene.SetActive(true);
		SetExpression(charDarlene, darleneShock);

		// Show text
		StartDialogue("Darlene", "Cristel!");
		nextButton.SetActive(true);
		eventPos = 1;
	}

	IEnumerator EventOne()
	{
		nextButton.SetActive(false);

		charCristel.SetActive(true);
		SetExpression(charCristel, cristelNeutral);

		StartDialogue("Cristel", "Oh thank god—");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 2;
	}

	IEnumerator EventTwo()
	{
		nextButton.SetActive(false);



		charDarlene.SetActive(false);

		charMarc.SetActive(true);
		SetExpression(charMarc, marcSmile);

		RectTransform marcRect = charMarc.GetComponent<RectTransform>();
		Vector2 endPos = marcRect.anchoredPosition;

		yield return StartCoroutine(
			SlideCharacter(charMarc, endPos + new Vector2(800, 0), endPos, 0.5f)
		);

		StartDialogue("Marc", "Look who finally decided to respawn.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 3;
	}

	IEnumerator EventThree()
	{
		nextButton.SetActive(false);

		StartDialogue("Cristel", "Not funny.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 4;
	}

	IEnumerator EventFour()
	{
		nextButton.SetActive(false);

		charMarc.SetActive(false);
		charRaven.SetActive(true);
		SetExpression(charRaven, ravenNeutral);

		RectTransform ravenRect = charRaven.GetComponent<RectTransform>();
		Vector2 endPos = ravenRect.anchoredPosition;

		yield return StartCoroutine(
			SlideCharacter(charRaven, endPos + new Vector2(800, 0), endPos, 0.5f)
		);

		StartDialogue("Raven", "You took longer than usual.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 5;
	}

	IEnumerator EventFive()
	{
		nextButton.SetActive(false);

		StartDialogue("Cristel", "Usual?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 6;
	}

	IEnumerator EventSix()
	{
		nextButton.SetActive(false);

		charRaven.SetActive(false);
		charCristel.SetActive(false);

		StartDialogue("", "The classroom feels… rearranged.\n\nLike someone tried to remember it instead of actually remembering.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 7;
	}

	IEnumerator EventSeven()
	{
		nextButton.SetActive(false);

		charCristel.SetActive(true);

		StartDialogue("Cristel", "…Did we leave it like this?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 8;
	}

	IEnumerator EventEight()
	{
		nextButton.SetActive(false);

		charMarc.SetActive(true);
		SetExpression(charMarc, marcNeutral);

		StartDialogue("Marc", "I don’t remember putting name tags on chairs.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 9;
	}

	IEnumerator EventNine()
	{
		nextButton.SetActive(false);

		charMarc.SetActive(false);
		charDarlene.SetActive(true);
		SetExpression(charDarlene, darleneNeutral);

		StartDialogue("Darlene", "Maybe it’s for something?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 10;
	}

	IEnumerator EventTen()
	{
		nextButton.SetActive(false);

		charMarc.SetActive(false);

		charDarlene.SetActive(false);

		charRaven.SetActive(true);
		

		StartDialogue("Raven", "…There’s a message.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 11;
	}

	IEnumerator EventEleven()
	{
		nextButton.SetActive(false);

		StartDialogue("Raven", "Raven is always with Darlene.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 12;
	}

	IEnumerator EventTwelve()
	{
		nextButton.SetActive(false);

		StartDialogue("Raven", "Marc is always next to Cristel.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 13;
	}

	IEnumerator EventThirteen()
	{
		nextButton.SetActive(false);

		StartDialogue("Raven", "Kuh is everywhere.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 14;
	}
	IEnumerator EventFourteen()
	{
		nextButton.SetActive(false);

		charRaven.SetActive(false);
		charMarc.SetActive(true);

		StartDialogue("Marc", "Wow. That clears up absolutely nothing.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 15;
	}

	IEnumerator EventFifteen()
	{
		nextButton.SetActive(false);

		StartDialogue("Cristel", "No… it does.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 16;
	}

	IEnumerator EventSixteen()
	{
		nextButton.SetActive(false);

		charCristel.SetActive(false);
		charMarc.SetActive(false);

		StartDialogue("", "This is easy.\n\nI know them.\n\nI know where everyone belongs.\n\nRight?");

		yield return WaitForText();

		// ✅ NOW show button instead of loading scene
		nextButton.SetActive(true);
		eventPos = 17; // NEW STATE
	}

	IEnumerator EventSeventeen()
	{
		nextButton.SetActive(false);

		CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();
		fadeOut.SetActive(true);

		float t = 0;

		while (t < 1)
		{
			t += Time.deltaTime;
			fadeCanvas.alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds(0.3f);

		SceneManager.LoadScene("ChairPuzzleScene");
	}







	IEnumerator Event20()
	{
		charCristel.SetActive(true);
		SetExpression(charCristel, cristelNeutral);
		StartDialogue("Cristel", "Oh…\n\n…right.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 21;
	}

	IEnumerator Event21()
	{
		nextButton.SetActive(false);

		StartDialogue("Cristel", "Kuh never sat still.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 22;
	}

	IEnumerator Event22()
	{

		charRaven.SetActive(true);
		SetExpression(charRaven, ravenNeutral);
		nextButton.SetActive(false);

		StartDialogue("Raven", "…You remembered.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 23;
	}

	IEnumerator Event23()
	{
		charRaven.SetActive(false);
		charCristel.SetActive(false);
		nextButton.SetActive(false);

		StartDialogue("", "For a second,\nthe room feels… real again.");
		yield return WaitForText();

		nextButton.SetActive(true);
		// continue story here
		eventPos = 50; // instead of stopping
	}














	IEnumerator Event30()
	{
		charKuh.SetActive(true);
		SetExpression(charKuh, kuhNeutral);
		nextButton.SetActive(false);

		StartDialogue("Kuh", "…Funny.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 31;
	}

	IEnumerator Event31()
	{
		nextButton.SetActive(false);

		StartDialogue("Kuh", "I don’t remember sitting there.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 32;
	}

	IEnumerator Event32()
	{
		nextButton.SetActive(false);

		StartDialogue("Kuh", "Or… anywhere, really.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 33;
	}

	IEnumerator Event33()
	{
		charKuh.SetActive(false);
		nextButton.SetActive(false);

		StartDialogue("", "The silence stretches.\n\nSomething is off.");
		yield return WaitForText();

		nextButton.SetActive(true);

		eventPos = 50;
	}









	IEnumerator Event40()
	{
		nextButton.SetActive(false);

		StartDialogue("", "…No.\n\nThis isn’t right.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 41;
	}

	IEnumerator Event41()
	{
		nextButton.SetActive(false);

		StartDialogue("", "None of this is right.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 42;
	}

	IEnumerator Event42()
	{
		nextButton.SetActive(false);

		StartDialogue("", "Let’s try again.");
		yield return WaitForText();

		SceneManager.LoadScene("ChairPuzzleScene");
	}

	IEnumerator WaitForText()
	{
		float timer = 0f;

		while (textLength < currentTextLength)
		{
			timer += Time.deltaTime;

			// failsafe: break after 5 seconds
			if (timer > 5f)
			{
				Debug.LogWarning("Text timeout triggered!");
				break;
			}

			yield return null;
		}

		yield return new WaitForSeconds(0.05f);
	}

	void SetExpression(GameObject character, Sprite expression)
	{
		if (character == null)
		{
			Debug.LogError("Character is NULL!");
			return;
		}

		if (expression == null)
		{
			Debug.LogError("Expression is NULL!");
			return;
		}

		var img = character.GetComponent<UnityEngine.UI.Image>();

		if (img == null)
		{
			Debug.LogError(character.name + " has NO Image component!");
			return;
		}

		img.sprite = expression;
	}

	void StartDialogue(string speaker, string line)
	{
		mainTextObject.SetActive(true);
		charName.GetComponent<TMPro.TMP_Text>().text = speaker;

		TextCreator.runTextPrint = false;

		textToSpeak = line;

		// 🔥 SEND TEXT TO TEXTCREATOR
		TextCreator.fullText = textToSpeak;
		TextCreator.charCount = 0;

		currentTextLength = textToSpeak.Length;
		textLength = 0;

		TextCreator.runTextPrint = true;
	}




	IEnumerator ApproveDialogue()
	{
		StartDialogue("Cristel", "Oh…\n\n…right.");
		yield return WaitForText();

		StartDialogue("Cristel", "Kuh never sat still.");
		yield return WaitForText();

		StartDialogue("Raven", "…You remembered.");
		yield return WaitForText();

		StartDialogue("", "For a second,\nthe room feels… real again.");
		yield return WaitForText();

		nextButton.SetActive(true);
	}

	IEnumerator DisapproveDialogue()
	{
		StartDialogue("Kuh", "…Funny.");
		yield return WaitForText();

		StartDialogue("Kuh", "I don’t remember sitting there.");
		yield return WaitForText();

		StartDialogue("Kuh", "Or… anywhere, really.");
		yield return WaitForText();

		StartDialogue("", "The silence stretches.\n\nSomething is off.");
		yield return WaitForText();

		nextButton.SetActive(true);
	}

	IEnumerator GameOverDialogue()
	{
		StartDialogue("", "…No.\n\nThis isn’t right.");
		yield return WaitForText();

		StartDialogue("", "None of this is right.");
		yield return WaitForText();

		StartDialogue("", "Let’s try again.");
		yield return WaitForText();

		// 🔁 Restart puzzle
		SceneManager.LoadScene("ChairPuzzleScene");
	}




	IEnumerator Event50()
	{
		nextButton.SetActive(false);

		StartDialogue("", "Then the intercom clicks.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 51;
	}

	IEnumerator Event51()
	{
		nextButton.SetActive(false);

		StartDialogue("", "For a moment—\nit feels normal.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 52;
	}

	IEnumerator Event52()
	{
		nextButton.SetActive(false);

		StartDialogue("Intercom", "Angelus Domini nuntiavit Mariae…");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 53;
	}

	IEnumerator Event53()
	{
		nextButton.SetActive(false);

		charDarlene.SetActive(true);
		SetExpression(charDarlene, darleneNeutral);

		StartDialogue("Darlene", "…It’s late.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 54;
	}

	IEnumerator Event54()
	{
		nextButton.SetActive(false);

		charDarlene.SetActive(false);
		charKuh.SetActive(true);
		SetExpression(charKuh, kuhNeutral);

		StartDialogue("Kuh", "but its too early for the prayer..");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 55;
	}

	IEnumerator Event55()
	{

		charKuh.SetActive(false);
		nextButton.SetActive(false);

		StartDialogue("Intercom", "…et concepit de Spiritu—");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 56;
	}

	IEnumerator Event56()
	{
		nextButton.SetActive(false);

		StartDialogue("Intercom", "Students.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 57;
	}

	IEnumerator Event57()
	{
		nextButton.SetActive(false);

		StartDialogue("Intercom", "…report to the rooftop.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 58;
	}

	IEnumerator Event58()
	{
		nextButton.SetActive(false);

		StartDialogue("Intercom", "…5:17 PM.");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 59;
	}

	IEnumerator Event59()
	{
		nextButton.SetActive(false);

		charCristel.SetActive(true);

		StartDialogue("Cristel", "…Again?");
		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 60;
	}

	IEnumerator Event60()
	{

		charCristel.SetActive(false);
		nextButton.SetActive(false);

		StartDialogue("", "The door unlocks.\n\nBut something else doesn’t.");
		yield return WaitForText();

		// 🎬 END CHAPTER
		fadeOut.SetActive(true);

		yield return new WaitForSeconds(1.5f);

		// SceneManager.LoadScene("NextScene"); ← future

		SceneManager.LoadScene(3);
	}

	public void NextButton()
	{
		if (eventPos == 1) StartCoroutine(EventOne());
		if (eventPos == 2) StartCoroutine(EventTwo());
		if (eventPos == 3) StartCoroutine(EventThree());
		if (eventPos == 4) StartCoroutine(EventFour());
		if (eventPos == 5) StartCoroutine(EventFive());

		if (eventPos == 6) StartCoroutine(EventSix());
		if (eventPos == 7) StartCoroutine(EventSeven());
		if (eventPos == 8) StartCoroutine(EventEight());
		if (eventPos == 9) StartCoroutine(EventNine());
		if (eventPos == 10) StartCoroutine(EventTen());
		if (eventPos == 11) StartCoroutine(EventEleven());
		if (eventPos == 12) StartCoroutine(EventTwelve());
		if (eventPos == 13) StartCoroutine(EventThirteen());
		if (eventPos == 14) StartCoroutine(EventFourteen());
		if (eventPos == 15) StartCoroutine(EventFifteen());
		if (eventPos == 16) StartCoroutine(EventSixteen());
		if (eventPos == 17) StartCoroutine(EventSeventeen());



		if (eventPos == 20) StartCoroutine(Event20());
		if (eventPos == 21) StartCoroutine(Event21());
		if (eventPos == 22) StartCoroutine(Event22());
		if (eventPos == 23) StartCoroutine(Event23());

		if (eventPos == 30) StartCoroutine(Event30());
		if (eventPos == 31) StartCoroutine(Event31());
		if (eventPos == 32) StartCoroutine(Event32());
		if (eventPos == 33) StartCoroutine(Event33());

		if (eventPos == 40) StartCoroutine(Event40());
		if (eventPos == 41) StartCoroutine(Event41());
		if (eventPos == 42) StartCoroutine(Event42());


		if (eventPos == 50) StartCoroutine(Event50());
		if (eventPos == 51) StartCoroutine(Event51());
		if (eventPos == 52) StartCoroutine(Event52());
		if (eventPos == 53) StartCoroutine(Event53());
		if (eventPos == 54) StartCoroutine(Event54());
		if (eventPos == 55) StartCoroutine(Event55());
		if (eventPos == 56) StartCoroutine(Event56());
		if (eventPos == 57) StartCoroutine(Event57());
		if (eventPos == 58) StartCoroutine(Event58());
		if (eventPos == 59) StartCoroutine(Event59());
		if (eventPos == 60) StartCoroutine(Event60());
	}


}
