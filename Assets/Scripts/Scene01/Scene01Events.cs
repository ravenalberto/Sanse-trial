using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Scene01Events : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fadeScreenIn;
    public GameObject fadeOut;
    public GameObject textBox;
    public GameObject mainTextObject;
    public GameObject nextButton;
    public TMP_Text charNameText;
    public GameObject darkOverlay; // For "bakit parang ang dilim bigla?"

    [Header("Backgrounds")]
    public GameObject bgLobby;

    [Header("Characters")]
    public GameObject charKuh;
    public GameObject charCristel;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip lobbyAmbience;
    public AudioClip suspenseMusic;
    public AudioClip glitchSound;
    public AudioClip footstepsSound;

    [Header("Settings")]
    [SerializeField] private int eventPos = 0;
    private int currentTextLength;
    private bool isTyping = false;

    [System.Serializable]
    public class DialogueLine
    {
        public string name;
        [TextArea(3, 10)] public string text;
        public bool showKuh;
        public bool showCristel;
        public AudioClip sfxToPlay;
        public bool triggerDarkness;
        public bool triggerGlitch;
        public bool triggerFadeOut;
    }

    public List<DialogueLine> dialogueList = new List<DialogueLine>();

    void Start()
    {
        // Initialize Scene
        mainTextObject.SetActive(false);
        nextButton.SetActive(false);
        textBox.SetActive(false);
        if (darkOverlay != null) darkOverlay.SetActive(false);

        SetupDialogue();
        StartCoroutine(OpeningSequence());
    }

    void Update()
    {
        if (TextCreator.charCount >= currentTextLength && isTyping)
        {
            isTyping = false;
            nextButton.SetActive(true);
        }
    }

    void SetupDialogue()
    {
        // LOBBY - Expanded for atmosphere
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Psst—Tetel!", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Ah! Kuh, andiyan ka pala! Halos atakihin ako sa puso sa 'yo.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "San ka na naman galing? Kanina pa dapat tayo nasa room. Nasa room na sila, 'di ba?", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Dapat nandun na nga eh… pero walang sumasagot sa taas. Kumatok ako, kinalampag ko na yung pinto, pero wala talaga.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Ha? Baka tulog lang sila? O baka naka-headset si Raven kaya hindi ka naririnig?", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Hindi eh… kumatok ako. Ilang beses. Sobrang tahimik talaga. As in... wala.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Tsaka... may narinig ako kanina habang paakyat ako. Parang may naglalakad sa hallway sa dulo.", showKuh = true, showCristel = true, sfxToPlay = footstepsSound });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Mabagal yung lakad, parang kinakaladkad yung paa. Pero nung tinignan ko—wala naman. Walang tao.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Uy stop… ang creepy mo na. Hindi nakakatawa, Kuh. Baka guard lang 'yun o yung utility.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Seryoso ako, Tetel. Iba yung pakiramdam dito ngayon. Parang... parang pinapanood tayo ng pader.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Wait… bakit parang ang dilim bigla? Kanina lang maliwanag pa yung sikat ng araw sa labas ah?", showKuh = true, showCristel = true, triggerDarkness = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "…Napansin mo rin? Parang hinigop ng hallway yung ilaw.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Wait—narinig mo ‘yon? Parang may static sa intercom...", showKuh = true, showCristel = true, triggerGlitch = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Tetel… bilis. Huwag na tayo magtagal dito. Tara na sa taas.", showKuh = true, showCristel = true, triggerFadeOut = true });
    }

    IEnumerator OpeningSequence()
    {
        if (bgmSource != null && lobbyAmbience != null)
        {
            bgmSource.clip = lobbyAmbience;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        yield return new WaitForSeconds(2);
        fadeScreenIn.SetActive(false);
        charCristel.SetActive(true);
        yield return new WaitForSeconds(1);

        PlayDialogue(0);
    }

    public void NextButton()
    {
        if (isTyping) return;

        eventPos++;
        if (eventPos < dialogueList.Count)
        {
            if (dialogueList[eventPos].triggerFadeOut)
            {
                StartCoroutine(TransitionToNextScene());
            }
            else
            {
                PlayDialogue(eventPos);
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void PlayDialogue(int index)
    {
        nextButton.SetActive(false);
        textBox.SetActive(true);
        mainTextObject.SetActive(true);

        DialogueLine line = dialogueList[index];

        charNameText.text = line.name;
        charKuh.SetActive(line.showKuh);
        charCristel.SetActive(line.showCristel);

        // Effects
        if (line.sfxToPlay != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(line.sfxToPlay);
        }

        if (line.triggerDarkness && darkOverlay != null)
        {
            StartCoroutine(FadeInDarkness());
            if (bgmSource != null && suspenseMusic != null)
            {
                bgmSource.Stop();
                bgmSource.clip = suspenseMusic;
                bgmSource.Play();
            }
        }

        if (line.triggerGlitch && sfxSource != null && glitchSound != null)
        {
            sfxSource.PlayOneShot(glitchSound);
            // You could trigger a camera shake or screen shake script here
        }

        // Text display
        TextCreator.fullText = line.text;
        TextCreator.charCount = 0;
        TextCreator.runTextPrint = true;

        currentTextLength = line.text.Length;
        isTyping = true;
    }

    IEnumerator FadeInDarkness()
    {
        darkOverlay.SetActive(true);
        CanvasGroup cg = darkOverlay.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            float elapsed = 0;
            while (elapsed < 2f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0, 1, elapsed / 2f);
                yield return null;
            }
        }
    }

    IEnumerator TransitionToNextScene()
    {
        nextButton.SetActive(false);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}