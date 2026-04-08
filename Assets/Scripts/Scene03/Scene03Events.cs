using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ChairPuzzleManager;

public class Scene03Events : MonoBehaviour
{

	public GameObject charKuh;
	public GameObject charCristel;
	public GameObject charDarlene;
	public GameObject charMarc;
	public GameObject charRaven;
	public GameObject textBox;


	public GameObject dialogueUI;
	public GameObject blockPuzzleUI;

	public BlockPuzzleManager puzzleManager;


	public int marcTrust = 0;
	public int marcStayPoints = 0;

	[SerializeField] GameObject vnUI;

	[SerializeField] GameObject choiceUI;

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
	[SerializeField] Sprite marcAngry;

	[SerializeField] Sprite ravenNeutral;

	[SerializeField] Sprite darleneNeutral;
	[SerializeField] Sprite darleneShock;


	[SerializeField] GameObject fadeScreenIn;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	void Start()
	{
		if (PuzzleState.scene3Result != "")
		{
			StartCoroutine(HandlePuzzleResult());
		}
		else
		{
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

		string result = PuzzleState.scene3Result;
		PuzzleState.scene3Result = "";

		vnUI.SetActive(true);
		nextButton.SetActive(true);

		if (result == "Win")
		{
			eventPos = 22;

		}
		else if (result == "Lose")
		{
			eventPos = 22;
		}
	}

	IEnumerator PlayPuzzle(GameObject puzzleUI, Func<bool> isDone)
	{
		dialogueUI.SetActive(false);
		puzzleUI.SetActive(true);

		yield return new WaitUntil(() => isDone());

		puzzleUI.SetActive(false);
		dialogueUI.SetActive(true);
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


	IEnumerator EventStarter()
	{
		yield return new WaitForSeconds(0.5f);

		// Show chapter title
		StartDialogue("", "CHAPTER 2\n\nPatterns That Shouldn’t Exist");

		yield return WaitForText();

		yield return new WaitForSeconds(1f);

		// Fade out before actual scene
		fadeOut.SetActive(true);

		CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();

		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime;
			fadeCanvas.alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);

		fadeOut.SetActive(false);

		// NOW start actual story
		StartCoroutine(EventOne());
	}

	IEnumerator EventOne()
	{
		nextButton.SetActive(false);

		
		SetExpression(charRaven, ravenNeutral);

		StartDialogue("", "We shouldn’t be going.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 2;
	}

	IEnumerator EventTwo()
	{
		nextButton.SetActive(false);

		StartDialogue("", "That’s the first thing I notice.\n\nNot fear.\n\nJust… wrong direction.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 3;
	}

	IEnumerator EventThree()
	{
		nextButton.SetActive(false);

		charCristel.SetActive(true);
		SetExpression(charCristel, cristelNeutral);

		StartDialogue("Cristel", "Come on… it’s probably just another clue.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 4;
	}

	IEnumerator EventFour()
	{
		nextButton.SetActive(false);

		charRaven.SetActive(true);
		SetExpression(charRaven, ravenNeutral);
		StartDialogue("Raven", "Since when do we follow instructions from a broken intercom?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 5;
	}

	IEnumerator EventFive()
	{
		nextButton.SetActive(false);
		charCristel.SetActive(false);

		charMarc.SetActive(true);
		SetExpression(charMarc, marcSmile);

		StartDialogue("Marc", "We followed worse.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 6;
	}



	IEnumerator EventSix()
	{
		nextButton.SetActive(false);
		

		charRaven.SetActive(true);
		SetExpression(charRaven, ravenNeutral);

		StartDialogue("Raven", "Yeah, like your ideas.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 7;
	}

	IEnumerator EventSeven()
	{
		nextButton.SetActive(false);

	

		StartDialogue("Marc", "Wow. Betrayal.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 8;
	}

	IEnumerator EventEight()
	{
		nextButton.SetActive(false);
		charMarc.SetActive(false);


		charDarlene.SetActive(true);
		SetExpression(charDarlene, darleneNeutral);

		StartDialogue("Darlene", "Can we not joke right now?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 9;
	}

	IEnumerator EventNine()
	{
		nextButton.SetActive(false);

		StartDialogue("Raven", "…Why the rooftop?");

		yield return WaitForText();

	

		nextButton.SetActive(true);
		eventPos = 10;
	}

	IEnumerator EventTen()
	{
		nextButton.SetActive(false);
		charDarlene.SetActive(false);

		charMarc.SetActive(true);
		SetExpression(charMarc, marcAngry);
		StartDialogue("Marc", "…Why not?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 11;
	}
	IEnumerator EventEleven()
	{
		nextButton.SetActive(false);
		charMarc.SetActive(false);



		charDarlene.SetActive(true);
		SetExpression(charDarlene, darleneNeutral);
		StartDialogue("Darlene", "Don’t.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 12;
	}

	IEnumerator EventTwelve()
	{
		nextButton.SetActive(false);
		charDarlene.SetActive(false);

		charCristel.SetActive(true);
		SetExpression(charCristel, cristelNeutral);
		StartDialogue("Cristel", "It’s just another puzzle. Like earlier.");

		yield return WaitForText();



		nextButton.SetActive(true);
		eventPos = 13;
	}
	IEnumerator EventThirteen()
	{
		nextButton.SetActive(false);
		StartDialogue("Raven", "…You keep saying that.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 14;
	}

	IEnumerator EventFourteen()
	{
		nextButton.SetActive(false);

		StartDialogue("Cristel", "Because it is.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 15;
	}

	IEnumerator EventFifteen()
	{
		nextButton.SetActive(false);

		StartDialogue("Raven", "Or because you don’t want it to be something else?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 16;
	}

	IEnumerator EventSixteen()
	{
		nextButton.SetActive(false);
		charCristel.SetActive(false);
		charRaven.SetActive(false);

		StartDialogue("", "[Silence lingers.]");



		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 17;
	}

	IEnumerator EventSeventeen()
	{
		nextButton.SetActive(false);
		StartDialogue("", "Too many variables.\n\nToo many assumptions.\n\nToo many… versions.");

		yield return WaitForText();


		nextButton.SetActive(true);
		eventPos = 18;
	}

	IEnumerator EventEightteen()
	{
		nextButton.SetActive(false);
		StartDialogue("", "I pull out my phone");

		yield return WaitForText();


		nextButton.SetActive(true);
		eventPos = 19;
	}

	IEnumerator EventNineteen()
	{
		nextButton.SetActive(false);

		charCristel.SetActive(true);
		SetExpression(charCristel, cristelNeutral);
		StartDialogue("Cristel", "…Really?");

		yield return WaitForText();


		nextButton.SetActive(true);
		eventPos =20;
	}


	IEnumerator EventTwenty()
	{
		nextButton.SetActive(false);

		charRaven.SetActive(true);
		SetExpression(charRaven, ravenNeutral);
		StartDialogue("Raven", "I think better when things are structured.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 21;
	}

	IEnumerator EventTwentyOne()
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

		yield return new WaitForSeconds(0.5f);

		// ✅ SAVE PROGRESS BEFORE SWITCHING
		PlayerPrefs.SetInt("Scene03_Event", 22);

		SceneManager.LoadScene("BlockPuzzleScene");
	}

	IEnumerator StartBlockPuzzle()
	{
		dialogueUI.SetActive(false);
		blockPuzzleUI.SetActive(true);

		yield return new WaitUntil(() => puzzleManager.puzzleFinished);

		blockPuzzleUI.SetActive(false);
		dialogueUI.SetActive(true);

		yield return StartCoroutine(EventTwentyTwo());
	}

	IEnumerator EventTwentyTwo()
	{
		nextButton.SetActive(false);

		charMarc.SetActive(true);
		SetExpression(charMarc, marcAngry);
		StartDialogue("Marc", "…You done?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 23; // ✅ FIXED
	}

	IEnumerator EventTwentyThree()
	{
		nextButton.SetActive(false);

		charMarc.SetActive(false);

		charDarlene.SetActive(true);
		SetExpression(charDarlene, darleneNeutral);

		StartDialogue("Darlene", "We’re asking you.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 24;
	}

	IEnumerator EventTwentyFour()
	{
		nextButton.SetActive(false);
		charDarlene.SetActive(false);

		charCristel.SetActive(true);
		SetExpression(charCristel, cristelNeutral);

		StartDialogue("Cristel", "What do you think?");

		yield return WaitForText();

		nextButton.SetActive(false);
		charCristel.SetActive(false);

		// 🔥 SHOW CHOICE
		choiceUI.SetActive(true);
	}

	public void ChoiceA()
	{
		marcTrust -= 1;
		marcStayPoints -= 1;

		StartCoroutine(AfterChoice());
	}

	public void ChoiceB()
	{
		marcTrust -= 1;

		StartCoroutine(AfterChoice());
	}

	public void ChoiceC()
	{
		marcTrust += 1;
		marcStayPoints += 1;

		StartCoroutine(AfterChoice());
	}

	public void ChoiceD()
	{
		// shared responsibility path
		marcTrust += 2;

		StartCoroutine(AfterChoice());
	}

	IEnumerator AfterChoice()
	{
		choiceUI.SetActive(false);
		nextButton.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		// 🎭 MARC REACTION BASED ON CHOICE
		charMarc.SetActive(true);

		if (marcTrust <= -1 && marcStayPoints <= -1)
		{
			// 🔴 Blamed
			SetExpression(charMarc, marcAngry);
			StartDialogue("Marc", "…Right. Of course it is.");
			yield return WaitForText();
		}
		else if (marcTrust < 0)
		{
			// 🟡 Neutral (but leaning negative)
			SetExpression(charMarc, marcNeutral);
			StartDialogue("Marc", "…That’s helpful.");
			yield return WaitForText();
		}
		else if (marcTrust >= 2)
		{
			// 🔥 Strong choice (shared responsibility)
			SetExpression(charMarc, marcNeutral);
			StartDialogue("Marc", "…Yeah. I guess we all did.");
			yield return WaitForText();

		}
		else
		{
			// 🟢 Balanced
			SetExpression(charMarc, marcNeutral);
			StartDialogue("Marc", "…It never is.");
			yield return WaitForText();
		}

		yield return new WaitForSeconds(0.3f);

		// 👉 CONTINUE TO YOUR NEW SCENE
		nextButton.SetActive(true);
		eventPos = 25;
	}

	IEnumerator EventTwentyFive()
	{
		nextButton.SetActive(false);
		charMarc.SetActive(false);

		StartDialogue("", "[Silence hangs for a second too long]");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 26;
	}

	IEnumerator EventTwentySix()
	{
		nextButton.SetActive(false);

		StartDialogue("", "[SFX: intercom crackle]");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 27;
	}

	IEnumerator EventTwentySeven()
	{
		nextButton.SetActive(false);

		StartDialogue("INTERCOM", "Angelus Domini nuntiavit Mariae…");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 28;
	}

	IEnumerator EventTwentyEight()
	{
		nextButton.SetActive(false);

		charKuh.SetActive(true);
		SetExpression(charKuh, kuhNeutral);

		StartDialogue("Kuh", "…Again?");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 29;
	}

	IEnumerator EventTwentyNine()
	{
		nextButton.SetActive(false);

		StartDialogue("INTERCOM", "…et concepit de—");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 30;
	}

	IEnumerator EventThirty()
	{
		nextButton.SetActive(false);

		StartDialogue("", "[STATIC CUT]");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 31;
	}

	IEnumerator EventThirtyOne()
	{
		nextButton.SetActive(false);

		StartDialogue("INTERCOM", "…Proceed.");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 32;
	}

	IEnumerator EventThirtyTwo()
	{
		nextButton.SetActive(false);

		StartDialogue("", "[Clock ticking returns, louder than before]");

		yield return WaitForText();

		nextButton.SetActive(true);
		eventPos = 33;
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
		if (eventPos == 18) StartCoroutine(EventEightteen());
		if (eventPos == 19) StartCoroutine(EventNineteen());
		if (eventPos == 20) StartCoroutine(EventTwenty());

		if (eventPos == 21) StartCoroutine(EventTwentyOne());
		if (eventPos == 22) StartCoroutine(EventTwentyTwo());


		if (eventPos == 23) StartCoroutine(EventTwentyThree());
		if (eventPos == 24) StartCoroutine(EventTwentyFour());

		if (eventPos == 25) StartCoroutine(EventTwentyFive());
		if (eventPos == 26) StartCoroutine(EventTwentySix());
		if (eventPos == 27) StartCoroutine(EventTwentySeven());
		if (eventPos == 28) StartCoroutine(EventTwentyEight());
		if (eventPos == 29) StartCoroutine(EventTwentyNine());
		if (eventPos == 30) StartCoroutine(EventThirty());
		if (eventPos == 31) StartCoroutine(EventThirtyOne());
		if (eventPos == 32) StartCoroutine(EventThirtyTwo());


	}


}
