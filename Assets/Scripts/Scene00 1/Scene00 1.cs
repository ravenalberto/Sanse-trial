using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene001 : MonoBehaviour
{
    private Dictionary<string, int> trustScores = new Dictionary<string, int>();



    public void AddTrust(string characterName, int amount)
    {
        if (!trustScores.ContainsKey(characterName)) trustScores[characterName] = 0;
        trustScores[characterName] += amount;
        Debug.Log($"<color=cyan>Trust Update:</color> {characterName} is now at {trustScores[characterName]}");
    }

    private Coroutine typingCoroutine;
    private Coroutine bgFadeCoroutine;
    private Coroutine fadeCoroutine;

	public GameObject choicePanelMarcReality;

	public float textSpeed = 0.02f;
    public bool skipMode = false;

    private DialogueLine currentLine;
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private Queue<DialogueLine> tempQueue = new Queue<DialogueLine>();
    private bool usingTempQueue = false;
    private bool isTyping = false;

    [Header("UI Components")]
    public GameObject textBox;
    public TMP_Text charNameText;
    public TMP_Text dialogueText;
    public GameObject nextButton;
    public GameObject choicePanelComfort; // Choice for comforting Cristel
    public GameObject choicePanelMarc;    // Choice for answering Marc
    public CanvasGroup fadeCanvasGroup;

    [Header("Backgrounds")]
    public GameObject currentBG;
    public GameObject complabBG;
    public GameObject classroomBG;
    public GameObject hallwayBG;
    public GameObject blackBG;
    public GameObject doorBG;
    public GameObject openDoorBG;

    [Header("Portraits")]
    public GameObject cristelNeutral; public GameObject cristelFrown; public GameObject cristelSmile;
    public GameObject marcLaugh; public GameObject marcNeutral; public GameObject marcChide;
    public GameObject kuhNeutral;
    public GameObject ravenNeutral;
    public GameObject darleneNeutral; public GameObject darleneSad;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip intercomCrackle;
    public AudioClip glitchSFX;
    public AudioClip bgFadeSFX;
    public AudioClip bellSFX;

    void Start()


    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        if (complabBG != null)
        {
            currentBG = complabBG;
            complabBG.SetActive(true);
        }

        if (textBox != null) textBox.SetActive(false);
        if (choicePanelComfort != null) choicePanelComfort.SetActive(false);
        if (choicePanelMarc != null) choicePanelMarc.SetActive(false);
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;

        EnqueueScene03();
        ShowNextLine();
    }

	public void OnChoice7Selected(int index)
	{
		choicePanelMarcReality.SetActive(false);

		usingTempQueue = true;
		tempQueue.Clear();

		if (index == 0)
		{
			tempQueue.Enqueue(new DialogueLine(
				"Cristel",
				"Yes, none of this is real. You’re inside a game.",
				cristelNeutral
			));

			tempQueue.Enqueue(new DialogueLine(
				"Darlene",
				"Wha– who are you? Kuya Marc? What is happening?",
				darleneSad
			));

			tempQueue.Enqueue(new DialogueLine(
				"Marc",
				"Looks like they want you to realize it all along..",
				marcNeutral
			));
		}
		else if (index == 1)
		{
			tempQueue.Enqueue(new DialogueLine(
				"Darlene",
				"Do what? Who are you?! What is happening?",
				darleneSad
			));

			tempQueue.Enqueue(new DialogueLine(
				"Marc",
				"It’s okay, first impressions are always awkward.",
				marcLaugh
			));
		}
		else
		{
			tempQueue.Enqueue(new DialogueLine(
				"Darlene",
				"Kuya? Who are you talking to?",
				darleneNeutral
			));

			tempQueue.Enqueue(new DialogueLine(
				"Marc",
				"I see, so you don’t want to show yourself yet huh.",
				marcNeutral
			));
		}

		// 🔥 IMPORTANT: continue flow
		tempQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE7_END]"));

		ShowNextLine();
	}




	void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);
        if (skipMode && !isTyping && !choicePanelComfort.activeSelf && !choicePanelMarc.activeSelf) ShowNextLine();
    }

    void EnqueueScene03()
    {

        
     
        // --- ACT I: ESCAPE BACK TO COMLAB ---
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Finally! Nakalaya din tayo!", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_DOOR]"));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Tignan nyo, pinto ba ng comlab natin yan?", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "Raven checks her phone."));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Oo nga! Wait! Hindi na 5:17! Pero 3:45pm na, late na tayo sa class ni sir.", ravenNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_OPENDOOR]"));
        dialogueQueue.Enqueue(new DialogueLine("", "A classmate opens the door."));
        dialogueQueue.Enqueue(new DialogueLine("Classmate", "Oy! Late nanaman kayo!"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Uy pre! Namiss kita grabe!", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Wow.. everyone’s here…", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Darlene’s eyes gaze on Kuh who was greeted by some, Cristel being asked where they went, and Raven ducking towards her desk."));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc, however, his back towards everyone, he’s looking out the window."));
        dialogueQueue.Enqueue(new DialogueLine("", "Darlene didn't pay him much mind. Maybe he just needs space. Like Cristel. Everything is fine now. Somehow they escaped."));
        dialogueQueue.Enqueue(new DialogueLine("", "But something still doesn't sit right. Darlene shakes her head. She sits near her desk. Everyone looked fine. But she worries about Marc."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "I wonder if he really is okay…", darleneSad));


        // --- ACT II: ATTEMPT TO REPAIR CRISTEL'S HEART ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_HALLWAY]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("", "The afternoon progresses. Darlene corners Cristel in the quiet hallway near the lockers to finally clear the heavy clouds between them."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Salamat nga pala kanina, Dar. At sa lahat. Sa paghahanap sa akin...", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Malamang hahanapin ka namin. Pamilya na tayo rito.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Actually... hindi ko na talaga alam minsan kung paano magpapatuloy.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "May mga araw na nararamdaman kong pabigat lang ako... na mas okay siguro pag wala na lang ako.", cristelFrown));

        // Choice 7: Comforting Cristel
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_COMFORT]"));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Siguro..", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel, naniniwala akong hanggang dulo magkakasama pa rin tayong lahat. Wag kayong mawalan ng hope.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Basta promise me, you’ll stick with us til the end. Okay?", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Darlene extends her pinky finger. For a moment, Cristel stares at it before slowly raising her own."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Promise, Dar.", cristelSmile));

        // --- ACT III: CLASSROOM ILLUSION / MARC'S PROVOCATION ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("", "They return to the classroom. The sunset leaks through the dust-mote windows, painting everything in an elegant, almost unreal, golden aura."));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc slowly drags a chair over and sits right next to Darlene's desk."));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "Dar.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Di ka naman siguro nauto na eto talaga yung nangyari, noh dar?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "What are you talking about, Kuya?", darleneSad));

		dialogueQueue.Enqueue(new DialogueLine("", "Marc turns towards you.", marcNeutral));

		dialogueQueue.Enqueue(new DialogueLine("Marc", "Right? None of them had actually listened to you.", marcNeutral));

		dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_7]"));

		// Choice 8: Responding to Marc's warnings
		//dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_MARC_WARN]"));

       // dialogueQueue.Enqueue(new DialogueLine("Marc", "Huwag mo kalilimutan, Dar... Ang pinto ay bukas lamang para sa naniniwala.", marcNeutral));
       // dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_GLITCH]"));
       // dialogueQueue.Enqueue(new DialogueLine("", "For a split second, the warm sunset background flickers violently into absolute darkness."));
       // dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_SCENE06]"));
    }

    public void OnComfortSelected(int index)
    {
        choicePanelComfort.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0) // Choice 7A: Comfort Warmly
        {
            AddTrust("Cristel", 15);
            
            tempQueue.Enqueue(new DialogueLine("Cristel", "Thank you, dar...", cristelSmile));
        }
        else // Choice 7B: Remind Value
        {
            AddTrust("Cristel", 10);
            tempQueue.Enqueue(new DialogueLine("Cristel", "Naappreciate ko yun, dar...", cristelSmile));
        }
        ShowNextLine();
    }

    public void OnMarcChoiceSelected(int index)
    {
        choicePanelMarc.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0) // Choice 8A: Defend Reality
        {
            AddTrust("Marc", -10);
            
            tempQueue.Enqueue(new DialogueLine("Marc", "Totoong mundo? Na 3:45 PM bigla pagkatapos ng lahat ng nangyari? Desperado ka rin para sa happy ending.", marcChide));
        }
        else // Choice 8B: Doubt Reality
        {
            AddTrust("Marc", 15);
            
            tempQueue.Enqueue(new DialogueLine("Marc", "Sana nagkakamali lang ako, Dar. Pero masama talaga ang kutob ko rito.", marcNeutral));
        }
        ShowNextLine();
    }

    private GameObject darleneGlareOrSad()
    {
        return darleneSad != null ? darleneSad : darleneNeutral;
    }

    public void OnNextClick()
    {
		if (choicePanelMarcReality.activeSelf ||
	choicePanelComfort.activeSelf ||
	choicePanelMarc.activeSelf)
		{
			return; // 🚨 BLOCK ALL DIALOGUE INPUT
		}
		ShowNextLine();
    }

    void ShowNextLine()
    {
        if (dialogueQueue.Count == 0 && tempQueue.Count == 0) return;
		if (choicePanelMarcReality.activeSelf ||
	        choicePanelComfort.activeSelf ||
	        choicePanelMarc.activeSelf)
		{
			return; // stop VN while choosing
		}

		DialogueLine line;
        if (usingTempQueue && tempQueue.Count > 0)
        {
            line = tempQueue.Dequeue();
            if (tempQueue.Count == 0 && line.speaker != "SYSTEM") usingTempQueue = false;
        }
        else
        {
            line = dialogueQueue.Dequeue();
        }

        if (line.speaker == "SYSTEM")
        {
            if (!HandleSystemCommand(line.text)) ShowNextLine();
            return;
        }

        currentLine = line;
        if (textBox != null) textBox.SetActive(true);

        HideAllPortraits();
        if (line.portrait != null) line.portrait.SetActive(true);
        charNameText.text = line.speaker;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private bool HandleSystemCommand(string command)
    {
        switch (command)
        {
            case "[CHOICE_COMFORT]":
                choicePanelComfort.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[CHOICE_MARC_WARN]":
                choicePanelMarc.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[SFX_INTERCOM]":
                if (sfxSource && intercomCrackle) sfxSource.PlayOneShot(intercomCrackle);
                return false;
            case "[SFX_GLITCH]":
                if (sfxSource && glitchSFX) sfxSource.PlayOneShot(glitchSFX);
                return false;
            case "[FADE_OUT]":
                StartFade(1.0f);
                return true;
            case "[FADE_IN]":
                StartFade(0.0f);
                return true;
			case "[CHOICE_7]":
				choicePanelMarcReality.SetActive(true);
				nextButton.SetActive(false);
				return true;
			case "[CHOICE7_END]":
				SceneManager.LoadScene("Scene06");
				return true;
			case "[BG_COMPLAB]": StartBGTransition(complabBG); return false;
            case "[BG_CLASSROOM]": StartBGTransition(classroomBG); return false;
            case "[BG_HALLWAY]": StartBGTransition(hallwayBG); return false;
            case "[BG_BLACK]": StartBGTransition(blackBG); return false;
            case "[BG_DOOR]": StartBGTransition(doorBG); return false;
            case "[BG_OPENDOOR]": StartBGTransition(openDoorBG); return false;
            case "[GOTO_SCENE06]":
                SceneManager.LoadScene("Scene06");
                return true;
            default: return false;
        }
    }

    void StartBGTransition(GameObject newBG)
    {
        if (newBG == currentBG) return;
        if (bgFadeCoroutine != null) StopCoroutine(bgFadeCoroutine);
        bgFadeCoroutine = StartCoroutine(FadeBackground(newBG));
    }

    IEnumerator FadeBackground(GameObject newBG)
    {
        float duration = 0.5f;
        if (bgFadeSFX != null && sfxSource != null) sfxSource.PlayOneShot(bgFadeSFX);

        if (currentBG != null)
        {
            CanvasGroup oldCG = currentBG.GetComponent<CanvasGroup>();
            if (oldCG != null)
            {
                float t = 0;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    oldCG.alpha = 1 - (t / duration);
                    yield return null;
                }
                oldCG.alpha = 0;
            }
            currentBG.SetActive(false);
        }

        newBG.SetActive(true);
        CanvasGroup newCG = newBG.GetComponent<CanvasGroup>();
        if (newCG != null)
        {
            newCG.alpha = 0;
            float t2 = 0;
            while (t2 < duration)
            {
                t2 += Time.deltaTime;
                newCG.alpha = t2 / duration;
                yield return null;
            }
            newCG.alpha = 1;
        }

        currentBG = newBG;
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    IEnumerator FadeRoutine(float targetAlpha)
    {
        float duration = 0.8f;
        float start = fadeCanvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha, elapsed / duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
        ShowNextLine();
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        nextButton.SetActive(false);
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(skipMode ? 0.001f : textSpeed);
        }
        isTyping = false;
        nextButton.SetActive(true);
    }

    void DisableAllBGs()
    {
        if (complabBG) complabBG.SetActive(false);
        if (classroomBG) classroomBG.SetActive(false);
        if (hallwayBG) hallwayBG.SetActive(false);
        if (blackBG) blackBG.SetActive(false);
        currentBG = null;
    }

    void HideAllPortraits()
    {
        GameObject[] ports = { cristelNeutral, cristelFrown, cristelSmile, marcLaugh, marcNeutral, marcChide, kuhNeutral, ravenNeutral, darleneNeutral, darleneSad };
        foreach (var p in ports) if (p) p.SetActive(false);
    }
}