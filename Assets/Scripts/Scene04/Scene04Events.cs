using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Scene04Events : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fadeOutOverlay;
    public GameObject textBox;
    public GameObject charName;
    public GameObject nextButton;
    public Image hallwayBackgroundImage;
    public GameObject[] chanceIndicators;

    [Header("VN & Choice UI")]
    public GameObject choiceUI;
    public TMP_Text[] choiceTexts;
    public GameObject vnUI;

    [Header("Maze Controls")]
    public GameObject leftButton;
    public GameObject rightButton;

    [Header("Characters")]
    public GameObject charDarlene;
    public GameObject charCristel;
    public GameObject charMarc;
    public GameObject charRaven;

    [Header("Maze Logic")]
    public Sprite[] hallwaySprites;
    private int mazeStage = 0;
    private int chances = 3;
    private bool canInteract = false;
    private bool isGameOver = false;

    [Header("Game State")]
    public int coreMemoriesUnlocked = 0;
    public float typingSpeedOverride = 0.08f;

    private Image cachedFadeImage;
    private bool nextButtonPressed = false; // Tracks if the player clicked Next

    // Pattern: Right, Right, Left, Right, Right, Left, Right
    private readonly bool[] solutionIsRight = { true, true, false, true, true, false, true };

    void Start()
    {
        if (fadeOutOverlay != null)
        {
            cachedFadeImage = fadeOutOverlay.GetComponent<Image>();
            fadeOutOverlay.SetActive(false);
            SetOverlayAlpha(1f);
        }

        if (nextButton != null) nextButton.SetActive(false);
        if (choiceUI != null) choiceUI.SetActive(false);
        if (leftButton != null) leftButton.SetActive(false);
        if (rightButton != null) rightButton.SetActive(false);

        MakeButtonTransparent(leftButton);
        MakeButtonTransparent(rightButton);

        if (hallwayBackgroundImage != null) hallwayBackgroundImage.raycastTarget = false;

        SetCharactersActive(true, true, true, true);
        UpdateChanceUI();

        // Start the intro sequence
        StartCoroutine(IntroSequence());
    }

    // Helper: Wait for text, show button, wait for click
    IEnumerator WaitAndClick()
    {
        yield return WaitForText();

        if (nextButton != null)
        {
            nextButton.SetActive(true);
            nextButton.transform.SetAsLastSibling();
        }

        nextButtonPressed = false;
        yield return new WaitUntil(() => nextButtonPressed);

        if (nextButton != null) nextButton.SetActive(false);
        nextButtonPressed = false;
    }

    // This should be linked to the "Next" button in the Inspector
    public void OnNextButtonPress()
    {
        nextButtonPressed = true;
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1f);

        StartDialogue("", "Ang bigat ng hangin dito... amoy lumang papel at may iba pang amoy na 'di ko maintindihan. Every breath feels like swallowing dust.");
        yield return WaitAndClick();

        StartDialogue("Tetel", "Nag-click yung pinto sa likod natin. Sinubukan ko yung handle pero ayaw talaga... I don't know, guys. The hallway ahead feels... wrong. Sobrang mali.");
        yield return WaitAndClick();

        StartDialogue("Raven", "Lahat naman mali, Tetel. We're literally walking into a void. My compass is spinning and the shadows... hindi sila sumasabay sa ilaw.");
        yield return WaitAndClick();

        // Continue directly to Event Two dialogue
        StartDialogue("Marc", "Huy, tignan niyo yung bright side. At least wala tayo sa room ni Prof. Lim para tapusin yung 20-page assignment. Remember that? Yung spatial anomalies?");
        yield return WaitAndClick();

        StartDialogue("", "Napatingin ako kay Marc. Sinusubukan niyang mag-joke, pero nanginginig yung mga kamay niya. He keeps checking his watch kahit tumigil na 'to kanina pa.");
        yield return WaitAndClick();

        StartDialogue("Darlene", "Marc, mas pipiliin ko pa yung 100-page assignment at isang buwan na detention kaysa dito. At least yung library, hindi humihinga sa batok mo.");
        yield return WaitAndClick();

        // Now trigger the transition to Maze POV
        StartCoroutine(TransitionToPOVMode());
    }

    IEnumerator TransitionToPOVMode()
    {
        if (fadeOutOverlay != null && cachedFadeImage != null)
            yield return StartCoroutine(FadeEffect(1f, 1f));

        SetCharactersActive(false, false, false, false);
        if (vnUI != null) vnUI.SetActive(false);

        mazeStage = 0;
        UpdateHallwaySprite();

        if (fadeOutOverlay != null && cachedFadeImage != null)
            yield return StartCoroutine(FadeEffect(0f, 1f));

        StartCoroutine(PlayStageDialogue(mazeStage));
    }

    IEnumerator PlayStageDialogue(int stage)
    {
        if (vnUI != null) vnUI.SetActive(true);
        canInteract = false;

        if (stage == 0)
        {
            StartDialogue("", "Bumabalik yung isip ko sa school. Siguro defense mechanism ko lang 'to, para makaramdam ng normal kahit papaano.");
            yield return WaitAndClick();
            StartDialogue("Darlene", "Marc, naalala mo yung deadline na na-miss natin dahil lang sa pagtatalo kung saan kakain? Napunta tayo doon sa may lumang tulay.");
            yield return WaitAndClick();
            StartDialogue("Marc", "Huwag mo nang ipaalala. Raven almost killed us both. Hawak na niya yung final draft pero ayaw isend hangga't 'di nauubos yung milkshake niya.");
            yield return WaitAndClick();
            StartDialogue("Raven", "Dapat lang. We got a C dahil yung 'shortcut' mo ay mahabang lakad lang pala papunta sa saradong cafe. You always think you know a better way.");
            yield return WaitAndClick();
            StartDialogue("Tetel", "Guys, please... focus. Parang yung mahabang lakad na yun yung ginagawa natin ngayon. The geometry is... repeating. Tignan niyo yung mga bitak sa sahig.");
            yield return WaitAndClick();
            StartDialogue("", "Huminga ako nang malalim. Kailangan ko silang i-lead dito. Kung hindi kami magkakaisa, mawawala kami sa katahimikan.");
            yield return WaitAndClick();
        }
        else if (stage == 1)
        {
            StartDialogue("", "Yung mga anino sa pader... para silang mga kamay na gustong mang-abot. They stretch longer than they should, humihila papunta sa dulo.");
            yield return WaitAndClick();
            StartDialogue("Raven", "Napansin niyo ba yung pag-flicker ng ilaw? Hindi siya random. 3 short, 2 long... para siyang sumasabay sa bawat hakbang natin.");
            yield return WaitAndClick();
            StartDialogue("Tetel", "Raven, tumigil ka na! Pinapalala mo lang eh. Hindi na ako makahinga kapag ganyan yung sinasabi mo. My chest feels tight... like someone is watching us.");
            yield return WaitAndClick();
            StartDialogue("Marc", "Ganyan lang talaga si Raven, Tetel. Over-analyzing things para 'di matakot. Pero yeah... it is creepy. Parang horror movie noong 80s.");
            yield return WaitAndClick();
            StartDialogue("Darlene", "Isang horror movie kung saan tayo yung mabibiktima kapag 'di tayo gumalaw. Kailangan nating maging sharp.");
            yield return WaitAndClick();
            StartDialogue("", "Kailangan kong maramdaman kung gaano sila katakot. Baka may napansin sila na hindi ko nakita. Lumalakas yung amoy ng ozone.");
            yield return WaitAndClick();
        }
        else if (stage == 2)
        {
            StartDialogue("", "Sa gitna ng katahimikan, naisip ko bigla... tayo. Kung paano tayo nauwi sa ganito. Yung mga maliliit na bagay noon na parang ang laki ngayon.");
            yield return WaitAndClick();
            StartDialogue("Tetel", "Darlene... tingin mo ba babalik pa tayo sa dati? Bago yung mga away? Bago tayo tumigil mag-usap tungkol sa nangyari sa tulay?");
            yield return WaitAndClick();
            StartDialogue("Marc", "Tetel, hindi ito yung tamang oras para sa 'The Talk'. Kailangan nating magpatuloy bago tuluyang mamatay yung ilaw.");
            yield return WaitAndClick();
            StartDialogue("Raven", "Actually, baka ito na yung tamang oras. Kung mamamatay man tayo dito sa paulit-ulit na loop, ayoko namang may dala-dalang sikreto. We all know the bridge wasn't an accident.");
            yield return WaitAndClick();
            StartDialogue("Darlene", "Ang relationships ay nabubuo sa trust. At ngayon, trust na lang ang meron tayo para hindi tayo magkawatak-watak.");
            yield return WaitAndClick();
            StartDialogue("", "Tinitigan ko sila. Tetel's eyes are glassy. Marc won't look at Raven. Bawat salita, parang naglalakad ako sa basag na salamin.");
            yield return WaitAndClick();
        }

        ShowConversationChoices(stage);
    }

    void ShowConversationChoices(int stage)
    {
        if (vnUI != null) vnUI.SetActive(false);
        if (choiceUI != null) choiceUI.SetActive(true);

        switch (stage)
        {
            case 0:
                SetChoices("Tanungin si Marc tungkol sa 'notes'", "Hingin ang logical path kay Raven", "Kamustahin si Tetel", "Masdan ang mga anino");
                break;
            case 1:
                SetChoices("I-question ang mga ilaw", "Itanong kung may narinig sila", "Subukang i-comfort si Tetel", "Paunahin si Marc");
                break;
            case 2:
                SetChoices("Tanungin si Tetel tungkol sa 'feeling' niya", "Itanong kay Marc kung tiwala siya", "Paanalyze kay Raven yung draft", "Tiwala sa sariling intuition");
                break;
            default:
                SetChoices("Kanan", "Kaliwa", "Maghintay", "Tumakbo");
                break;
        }
    }

    public void SelectChoice(int index)
    {
        if (choiceUI != null) choiceUI.SetActive(false);
        StartCoroutine(HandleChoiceReaction(index));
    }

    IEnumerator HandleChoiceReaction(int choiceIndex)
    {
        if (vnUI != null) vnUI.SetActive(true);
        string speaker = "Darlene";
        string line = "...";

        bool isRight = solutionIsRight[mazeStage];

        // Investigate stage logic
        if (mazeStage == 0) // Solution is RIGHT (Kanan)
        {
            switch (choiceIndex)
            {
                case 0: // Marc
                    speaker = "Marc";
                    line = "Yung notes... may scribble ako sa margin. 'Always lean towards the source of the draft.' Wait, nararamdaman ko yung hangin... galing doon.";
                    UnlockCoreMemory("The Library Incident");
                    break;
                case 1: // Raven
                    speaker = "Raven";
                    line = "Kung pakikinggan mo yung echo, mas 'open' yung sound waves sa kabilang side. Yung isa, parang dead end agad. Logic dictates the clearer path.";
                    break;
                case 2: // Tetel
                    speaker = "Tetel";
                    line = "Darlene, parang may umiiyak sa may kaliwang hallway... Ayokong lumapit doon. Parang may humihila sa akin palayo.";
                    break;
                case 3: // Observe
                    speaker = "Darlene";
                    line = "May mga scratches sa sahig... lahat sila nakaturo sa iisang direction. Parang may kinaladkad papunta doon.";
                    break;
            }
        }
        else if (mazeStage == 1) // Solution is RIGHT (Kanan)
        {
            switch (choiceIndex)
            {
                case 0: // Lights
                    speaker = "Raven";
                    line = "Ang pag-flicker ay 1-0-1 pattern. Sa binary, it stands for 'A'. Alpha. The beginning of the right choice? O baka literal na nakaturo lang yung pulse.";
                    break;
                case 1: // Noise
                    speaker = "Marc";
                    line = "Shh! May kumakalos sa pader sa may kaliwa. Parang may sumusunod sa atin sa loob mismo ng hollow blocks. Wag tayo doon.";
                    break;
                case 2: // Comfort
                    speaker = "Tetel";
                    line = "Salamat, Darlene. Pero yung mga mata... nakita mo ba? Maraming mga mata sa dilim ng kaliwang pasilyo. Tinitignan nila tayo.";
                    UnlockCoreMemory("The Whispers in the Hall");
                    break;
                case 3: // Marc Lead
                    speaker = "Marc";
                    line = "Sige, ako na mauuna. Pero bakit parang mas 'natural' yung liwanag sa kabilang side? Parang hindi siya peke.";
                    break;
            }
        }
        else if (mazeStage == 2) // Solution is LEFT (Kaliwa)
        {
            switch (choiceIndex)
            {
                case 0: // Tetel Gut
                    speaker = "Tetel";
                    line = "Bumabagal yung tibok ng puso ko kapag nakatingin ako sa kaliwa. Pero sa kanan? Parang may pressure sa dibdib ko.";
                    UnlockCoreMemory("Tetel's Vision");
                    break;
                case 1: // Marc Trust
                    speaker = "Marc";
                    line = "Naalala mo noong high school? Lagi tayong naliligaw kapag 'nag-kakanan' tayo sa forest park. Maybe kailangan nating gawin yung kabaligtaran.";
                    break;
                case 2: // Raven Suction
                    speaker = "Raven";
                    line = "Yung dust particles... tignan niyo. Hinihila sila papasok doon sa isang pinto. Natural suction usually leads to a larger atmospheric pressure area.";
                    break;
                case 3: // Intuition
                    speaker = "Darlene";
                    line = "Naaalala ko yung laging sinasabi ni Mama... 'Left is for life, Right is for regret.' Ewan ko kung bakit ko biglang naisip yun.";
                    break;
            }
        }
        else
        {
            speaker = isRight ? "Marc" : "Raven";
            line = isRight ? "Tara sa Kanan. Mas mukhang tama doon." : "Sa Kaliwa muna tayo, mukhang mas safe.";
        }

        StartDialogue(speaker, line);
        yield return WaitAndClick();

        canInteract = true;
        if (leftButton != null) { leftButton.SetActive(true); leftButton.transform.SetAsLastSibling(); }
        if (rightButton != null) { rightButton.SetActive(true); rightButton.transform.SetAsLastSibling(); }
    }

    void UnlockCoreMemory(string memoryName)
    {
        coreMemoriesUnlocked++;
        Debug.Log($"CORE MEMORY UNLOCKED: {memoryName} | Total: {coreMemoriesUnlocked}");
    }

    public void ChooseLeft() { if (canInteract) StartCoroutine(ProcessChoice(false)); }
    public void ChooseRight() { if (canInteract) StartCoroutine(ProcessChoice(true)); }

    IEnumerator ProcessChoice(bool choseRight)
    {
        canInteract = false;
        if (leftButton != null) leftButton.SetActive(false);
        if (rightButton != null) rightButton.SetActive(false);
        if (vnUI != null) vnUI.SetActive(false);

        float t = 0;
        Vector3 endScale = new Vector3(1.5f, 1.5f, 1.5f);
        Vector2 endPos = choseRight ? new Vector2(-300f, 0) : new Vector2(300f, 0);

        if (hallwayBackgroundImage != null)
        {
            while (t < 1f)
            {
                t += Time.deltaTime / 0.8f;
                hallwayBackgroundImage.rectTransform.localScale = Vector3.Lerp(Vector3.one, endScale, t);
                hallwayBackgroundImage.rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, endPos, t);
                yield return null;
            }
        }

        bool isCorrect = (choseRight == solutionIsRight[mazeStage]);

        if (!isCorrect)
        {
            chances--;
            UpdateChanceUI();
            if (chances == 2) yield return StartCoroutine(GlitchEffect(0.5f));
            else if (chances == 1) yield return StartCoroutine(SecondMistakeTwitchSequence());
            else { yield return StartCoroutine(GameOverSequence()); yield break; }
        }
        else
        {
            mazeStage++;
            if (mazeStage >= solutionIsRight.Length)
            {
                if (fadeOutOverlay != null && cachedFadeImage != null)
                    yield return StartCoroutine(FadeEffect(1f, 1f));
                SceneManager.LoadScene("Marc_TestScene");
                yield break;
            }
        }

        if (fadeOutOverlay != null && cachedFadeImage != null)
            yield return StartCoroutine(FadeEffect(1f, 0.5f));

        ResetHallwayPosition();
        UpdateHallwaySprite();

        if (fadeOutOverlay != null && cachedFadeImage != null)
            yield return StartCoroutine(FadeEffect(0f, 0.5f));

        StartCoroutine(PlayStageDialogue(mazeStage));
    }

    void ResetHallwayPosition()
    {
        if (hallwayBackgroundImage != null)
        {
            hallwayBackgroundImage.rectTransform.localScale = Vector3.one;
            hallwayBackgroundImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    IEnumerator WaitForText()
    {
        float timer = 0f;
        while (timer < 8f)
        {
            try
            {
                if (TextCreator.fullText != null && TextCreator.charCount >= TextCreator.fullText.Length)
                    break;
            }
            catch { break; }
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator FadeEffect(float targetAlpha, float duration)
    {
        if (fadeOutOverlay == null || cachedFadeImage == null) yield break;
        fadeOutOverlay.SetActive(true);
        fadeOutOverlay.transform.SetAsLastSibling();
        float startAlpha = cachedFadeImage.color.a;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            cachedFadeImage.color = new Color(cachedFadeImage.color.r, cachedFadeImage.color.g, cachedFadeImage.color.b, newAlpha);
            yield return null;
        }
        cachedFadeImage.color = new Color(cachedFadeImage.color.r, cachedFadeImage.color.g, cachedFadeImage.color.b, targetAlpha);
        if (targetAlpha <= 0) fadeOutOverlay.SetActive(false);
    }

    void SetOverlayAlpha(float alpha)
    {
        if (fadeOutOverlay != null && cachedFadeImage != null)
            cachedFadeImage.color = new Color(cachedFadeImage.color.r, cachedFadeImage.color.g, cachedFadeImage.color.b, alpha);
    }

    IEnumerator GlitchEffect(float duration)
    {
        if (hallwayBackgroundImage == null) yield break;
        float elapsed = 0;
        Vector2 originalPos = hallwayBackgroundImage.rectTransform.anchoredPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hallwayBackgroundImage.rectTransform.anchoredPosition = originalPos + new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
            hallwayBackgroundImage.color = (Random.value > 0.5f) ? Color.red : Color.white;
            yield return new WaitForSeconds(0.05f);
        }
        hallwayBackgroundImage.rectTransform.anchoredPosition = originalPos;
        hallwayBackgroundImage.color = Color.white;
    }

    IEnumerator SecondMistakeTwitchSequence()
    {
        if (hallwayBackgroundImage == null) yield break;
        float duration = 5.0f;
        float elapsed = 0;
        bool cristelHasFlashed = false;
        Vector2 originalPos = hallwayBackgroundImage.rectTransform.anchoredPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hallwayBackgroundImage.rectTransform.anchoredPosition = originalPos + new Vector2(Random.Range(-35f, 35f), Random.Range(-35f, 35f));
            hallwayBackgroundImage.color = (Random.value > 0.4f) ? Color.red : Color.black;
            if (!cristelHasFlashed && elapsed > 2.5f) { cristelHasFlashed = true; StartCoroutine(FlashCristelBriefly(0.3f)); }
            yield return new WaitForSeconds(0.02f);
        }
        hallwayBackgroundImage.rectTransform.anchoredPosition = originalPos;
        hallwayBackgroundImage.color = Color.white;
    }

    IEnumerator FlashCristelBriefly(float duration)
    {
        if (charCristel != null)
        {
            charCristel.SetActive(true); charCristel.transform.SetAsLastSibling();
            yield return new WaitForSeconds(duration); charCristel.SetActive(false);
        }
    }

    IEnumerator GameOverSequence()
    {
        isGameOver = true;
        if (vnUI != null) vnUI.SetActive(true);
        StartDialogue("SYSTEM", "<color=red><b>ERROR: DESTINATION NOT FOUND.</b></color>\n\nKinakain na kayo ng dilim...");
        yield return new WaitForSeconds(3f);
        if (fadeOutOverlay != null) yield return StartCoroutine(FadeEffect(1f, 1f));
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateHallwaySprite()
    {
        if (hallwaySprites != null && mazeStage < hallwaySprites.Length && hallwayBackgroundImage != null)
            hallwayBackgroundImage.sprite = hallwaySprites[mazeStage];
    }

    void SetCharactersActive(bool d, bool c, bool m, bool r)
    {
        if (charDarlene != null) charDarlene.SetActive(d);
        if (charCristel != null) charCristel.SetActive(c);
        if (charMarc != null) charMarc.SetActive(m);
        if (charRaven != null) charRaven.SetActive(r);
    }

    void UpdateChanceUI()
    {
        if (chanceIndicators == null) return;
        for (int i = 0; i < chanceIndicators.Length; i++)
            if (chanceIndicators[i] != null) chanceIndicators[i].SetActive(i < chances);
    }

    void StartDialogue(string speaker, string line)
    {
        try
        {
            if (textBox != null) textBox.SetActive(true);
            if (charName != null)
            {
                var tmp = charName.GetComponent<TMP_Text>();
                if (tmp != null) { tmp.text = speaker; tmp.raycastTarget = false; }
            }
            TextCreator.fullText = line;
            TextCreator.charCount = 0;
            TextCreator.runTextPrint = true;
        }
        catch { }
    }

    void MakeButtonTransparent(GameObject btnObj)
    {
        if (btnObj == null) return;
        Image img = btnObj.GetComponent<Image>();
        if (img != null) img.color = new Color(1, 1, 1, 0);
    }

    void SetChoices(string a, string b, string c, string d)
    {
        if (choiceTexts != null && choiceTexts.Length >= 4)
        {
            if (choiceTexts[0] != null) choiceTexts[0].text = a;
            if (choiceTexts[1] != null) choiceTexts[1].text = b;
            if (choiceTexts[2] != null) choiceTexts[2].text = c;
            if (choiceTexts[3] != null) choiceTexts[3].text = d;
        }
    }
}