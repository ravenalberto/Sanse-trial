using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Scene00Events : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fadeScreenIn;
    public GameObject glitchOverlay;
    public GameObject flashOverlay; // For white flashes
    public GameObject textBox;
    public TMP_Text charNameText;
    public TMP_Text dialogueText;
    public GameObject nextButton;
    public GameObject fadeOut;

    [Header("Character Portraits (Normal)")]
    public GameObject portraitCristel;
    public GameObject portraitKuh;
    public GameObject portraitMarc;
    public GameObject portraitRaven;
    public GameObject portraitDarlene;

    [Header("Character Portraits (Expressions)")]
    public GameObject portraitMarc_Laugh;
    public GameObject portraitCristel_Worried;
    public GameObject portraitKuh_Scared;
    public GameObject portraitDarlene_Smile;

    [Header("Object Cut-ins")]
    public GameObject clockObject; // The clock image that fades in/out
    public GameObject shadowsObject; // Visual for "long dark fingers"

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip bellSound;
    public AudioClip glitchSound;
    public AudioClip clockTickSound;
    public AudioClip heartbeatSound;
    public AudioClip flashSound;

    private int step = 0;
    private bool isTyping = false;
    private int currentTextLength;

    void Start()
    {
        // Initial UI State
        glitchOverlay.SetActive(false);
        if (flashOverlay != null) flashOverlay.SetActive(false);
        if (clockObject != null) clockObject.SetActive(false);
        if (shadowsObject != null) shadowsObject.SetActive(false);
        textBox.SetActive(false);
        nextButton.SetActive(false);
        if (fadeOut != null) fadeOut.SetActive(false);

        // Hide all portraits initially
        HideAllPortraits();

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        if (fadeScreenIn != null) fadeScreenIn.SetActive(true);
        yield return new WaitForSeconds(2f);

        HideAllPortraits();
        ShowDialogue("", "The hallway is packed with students in their yellow uniforms. The air is thick with the scent of floor wax and the collective exhaustion of a long school day.");

        if (fadeScreenIn != null) fadeScreenIn.SetActive(false);
    }

    public void OnNextClick()
    {
        if (isTyping) return;

        step++;
        switch (step)
        {
            case 1:
                HideAllPortraits();
                ShowDialogue("", "5:10 PM. The perfect time. The sun hits the corridor at just the right angle, turning everything into a warm, golden haze.");
                break;
            case 2:
                HideAllPortraits();
                if (portraitMarc != null) portraitMarc.SetActive(true);
                ShowDialogue("Marc", "Sinasabi ko sa inyo, kung hindi lang ako pinigilan ni Ma'am kanina, natapos ko yung exam in twenty minutes. Absolute speedrun record 'yun, promise.");
                break;
            case 3:
                HideAllPortraits();
                if (portraitRaven != null) portraitRaven.SetActive(true);
                ShowDialogue("Raven", "Anong speedrun? Labinlimang minuto ka lang tumititig sa kisame, Marc. Speedrun record of giving up 'yun, hindi exam.");
                break;
            case 4:
                HideAllPortraits();
                if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Hala! Sunog! Raven really just hit you with the 'Logic 101' card. Grabe siya, 'di man lang nag-preno.");
                break;
            case 5:
                HideAllPortraits();
                if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "To be fair, Marc did look like he was trying to communicate with ghosts para makuha yung answers. Kulang na lang mag-summon siya ng spirit ni San Sebastian.");
                break;
            case 6:
                HideAllPortraits();
                if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(true);
                else if (portraitMarc != null) portraitMarc.SetActive(true);
                ShowDialogue("Marc", "Grabe kayo sa akin. Nagre-reflect lang yung tao, 'di ba? Visualizing the success!");
                break;
            case 7:
                HideAllPortraits();
                if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(true);
                else if (portraitDarlene != null) portraitDarlene.SetActive(true);
                ShowDialogue("Darlene", "Guys, tama na 'yan. At least he tried, 'di ba? Anyway, check niyo muna gamit niyo. Ayokong bumalik at umakyat ng tatlong palapag dahil lang may nakalimutan kayong charger o payong.");
                break;
            case 8:
                HideAllPortraits();
                if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(true);
                else if (portraitMarc != null) portraitMarc.SetActive(true);
                ShowDialogue("Marc", "Check na lahat. My phone, my wallet, and my devastating good looks. Kumpleto na.");
                break;
            case 9:
                HideAllPortraits();
                if (portraitRaven != null) portraitRaven.SetActive(true);
                ShowDialogue("Raven", "So, dalawang bagay lang pala dala mo? Yung looks, debatable pa.");
                break;
            case 10:
                HideAllPortraits();
                if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Ouch! Brutal talaga ni Raven today. Gutom lang 'yan, tara na kasi sa labas! Fishball muna tayo bago umuwi.");
                break;
            case 11:
                HideAllPortraits();
                if (portraitRaven != null) portraitRaven.SetActive(true);
                ShowDialogue("Raven", "Mauuna na kami sa lobby ni Marc. Tignan lang namin kung siksikan na sa gate. Baka mahirapan tayo sumakay sa Recto kung magtatagal pa tayo rito.");
                break;
            case 12:
                HideAllPortraits();
                if (portraitDarlene != null) portraitDarlene.SetActive(true);
                ShowDialogue("Darlene", "Sandali! Wait for me! Kailangan ko lang ibalik 'tong book sa library—nakalimutan ko na nasa bag ko pala 'to. Sobrang overdue na nito.");
                break;
            case 13:
                HideAllPortraits();
                if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Sige, antayin na lang namin kayo rito sa tapat ng lockers. Bilisan mo, Darlene, baka mag-close na yung library.");
                break;
            case 14:
                HideAllPortraits();
                if (portraitDarlene != null) portraitDarlene.SetActive(true);
                ShowDialogue("Darlene", "Five minutes lang, promise! Don't leave without me, ha!");
                break;
            case 15:
                HideAllPortraits();
                ShowDialogue("", "The hallway suddenly feels much longer as soon as the rest of the group disappears around the corner.");
                break;
            case 16:
                HideAllPortraits();
                if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Hays, finally... akala ko hindi na matatapos yung last period. Sobrang high energy ng lahat ngayon, 'no? Siguro dahil Friday.");
                break;
            case 17:
                HideAllPortraits();
                if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Friday energy, Kuh. Lahat excited nang lumabas ng gate at mag-abang ng jeep pauwi. Gusto na lang nating lahat humiga at mag-cellphone.");
                break;
            case 18:
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                else if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "True. Pero parang... ang bilis dumilim today? Kanina lang ang liwanag pa.");
                break;
            case 19:
                StartCoroutine(GlitchTransition());
                break;
            case 20:
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                else if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "...Wait. Narinig mo 'yun?");
                break;
            case 21:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                else if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Ang alin?");
                break;
            case 22:
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                else if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Yung bell. Parang... iba yung tunog. Like a broken record. Or parang galing sa ilalim ng tubig na ewan.");
                break;
            case 23:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                else if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Baka sira lang yung speakers. Alam mo naman dito, minsan nagpaparamdam talaga yung sound system. Tara na, baka iwanan pa tayo nina Darlene.");
                break;
            case 24:
                HideAllPortraits();
                if (bgmSource != null) bgmSource.Stop();
                if (sfxSource != null && clockTickSound != null)
                {
                    sfxSource.clip = clockTickSound;
                    sfxSource.loop = true;
                    sfxSource.Play();
                }
                // Show Clock Cut-in
                if (clockObject != null) StartCoroutine(FadeInObject(clockObject));
                ShowDialogue("", "A sudden chill runs down the corridor, despite the warm sunset glow. The ticking of a clock becomes rhythmic and heavy.");
                break;
            case 25:
                HideAllPortraits();
                if (sfxSource != null && bellSound != null) sfxSource.PlayOneShot(bellSound);
                ShowDialogue("Intercom", "An-ge-lus... Do-mi-ni... nun-ti-a-vit... Ma-ri-ae...");
                break;
            case 26:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                else if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Wait, ang aga naman yata para sa prayer. 5:15 pa lang, 'di ba? Diba 6 PM dapat 'yun?");
                break;
            case 27:
                HideAllPortraits();
                if (clockObject != null) StartCoroutine(FadeOutObject(clockObject));
                if (shadowsObject != null) StartCoroutine(FadeInObject(shadowsObject));
                StartCoroutine(FlashEffect());
                ShowDialogue("", "The screen turns a deep, unnatural crimson. The shadows of the lockers stretch across the floor like long, dark fingers.");
                break;
            case 28:
                HideAllPortraits();
                if (sfxSource != null) sfxSource.Stop(); // Stop clock, start heartbeat
                if (sfxSource != null && heartbeatSound != null) sfxSource.PlayOneShot(heartbeatSound);
                ShowDialogue("", "The clock shudders. Time stops. 5:17 PM.");
                break;
            case 29:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                else if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "...5:17? Pero 5:13 lang kanina sa phone ko. Lowbat ba 'to?");
                break;
            case 30:
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                else if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Cristel? Bakit biglang tumahimik? Nasaan na yung ibang mga estudyante? Kanina lang ang ingay dito...");
                break;
            case 31:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                else if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Kuh? Stay close. Hindi ko na... hindi ko na makita yung dulo ng hallway. Parang may fog na ewan.");
                break;
            case 32:
                HideAllPortraits();
                if (shadowsObject != null) StartCoroutine(FadeOutObject(shadowsObject));
                ShowDialogue("", "And then, the silence isn't just quiet. It is heavy, pressing against their ears. As if the school itself has stopped breathing.");
                break;
            case 33:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                else if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Kuh, tara sa baba... baka andun sila sa lobby. Bilisan natin.");
                break;
            case 34:
                HideAllPortraits();
                ShowDialogue("", "The darkness swallows the hallway as we move toward the stairs. Every step feels like we are sinking deeper into a cold, unfamiliar world.");
                break;
            case 35:
                StartCoroutine(TransitionToScene01());
                break;
        }
    }

    void ShowDialogue(string name, string text)
    {
        if (textBox != null) textBox.SetActive(true);
        if (charNameText != null) charNameText.text = name;
        if (nextButton != null) nextButton.SetActive(false);

        TextCreator.fullText = text;
        TextCreator.charCount = 0;
        TextCreator.runTextPrint = true;

        currentTextLength = text.Length;
        StartCoroutine(WaitForText(currentTextLength));
    }

    IEnumerator WaitForText(int length)
    {
        isTyping = true;
        while (TextCreator.charCount < length)
        {
            yield return null;
        }
        isTyping = false;
        if (nextButton != null) nextButton.SetActive(true);
    }

    IEnumerator GlitchTransition()
    {
        if (nextButton != null) nextButton.SetActive(false);
        if (sfxSource != null && glitchSound != null) sfxSource.PlayOneShot(glitchSound);

        if (glitchOverlay != null) glitchOverlay.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        if (glitchOverlay != null) glitchOverlay.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        if (glitchOverlay != null) glitchOverlay.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        if (glitchOverlay != null) glitchOverlay.SetActive(false);

        OnNextClick();
    }

    IEnumerator FlashEffect()
    {
        if (flashOverlay != null)
        {
            if (sfxSource != null && flashSound != null) sfxSource.PlayOneShot(flashSound);
            flashOverlay.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            flashOverlay.SetActive(false);
        }
    }

    IEnumerator FadeInObject(GameObject obj)
    {
        if (obj != null) obj.SetActive(true);
        yield return null;
    }

    IEnumerator FadeOutObject(GameObject obj)
    {
        if (obj != null) obj.SetActive(false);
        yield return null;
    }

    IEnumerator TransitionToScene01()
    {
        isTyping = true;
        if (nextButton != null) nextButton.SetActive(false);
        if (fadeOut != null) fadeOut.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("HallwayScene01");
    }

    void HideAllPortraits()
    {
        if (portraitCristel != null) portraitCristel.SetActive(false);
        if (portraitKuh != null) portraitKuh.SetActive(false);
        if (portraitMarc != null) portraitMarc.SetActive(false);
        if (portraitRaven != null) portraitRaven.SetActive(false);
        if (portraitDarlene != null) portraitDarlene.SetActive(false);

        // Hide Expressions
        if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(false);
        if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(false);
        if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(false);
        if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(false);
    }
}