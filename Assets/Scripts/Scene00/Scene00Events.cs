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

    [Header("Backgrounds")]
    public GameObject backgroundHallway;
    public GameObject backgroundCanteen;
    public GameObject backgroundUncleJohns;
    public GameObject School;
    public GameObject Classroom;

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
    public GameObject clockObject;
    public GameObject shadowsObject;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip bellSound;
    public AudioClip intercomSound;
    public AudioClip glitchSound;
    public AudioClip clockTickSound;
    public AudioClip heartbeatSound;
    public AudioClip flashSound;
    public AudioClip flashbackBGM;

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
        if (backgroundHallway != null) backgroundHallway.SetActive(false);
        if (backgroundCanteen != null) backgroundCanteen.SetActive(false);
        if (backgroundUncleJohns != null) backgroundUncleJohns.SetActive(false);
        if (School != null) School.SetActive(false);
        if (Classroom != null) Classroom.SetActive(false);

        textBox.SetActive(false);
        nextButton.SetActive(false);
        if (fadeOut != null) fadeOut.SetActive(false);
        if (fadeScreenIn != null) fadeScreenIn.SetActive(false);


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
            // --- PROLOGUE: THE DISTORTION ---
            case 1:
                if (School != null)
                {
                    StartCoroutine(FadeInObject(School, 2.5f));
                    // Zoom in slowly: target scale 1.15x over 15 seconds for a subtle effect
                    StartCoroutine(ZoomInObject(School, 1.15f, 15f));
                }
                ShowDialogue("", "5:10 PM. The perfect time. The sun hits the corridor at just the right angle, turning everything into a warm, golden haze.");
                break;
            case 2:
                // Fixed: Background image is hallway and not the school
                if (backgroundHallway != null) StartCoroutine(FadeInObject(backgroundHallway, 2f));
                if (School != null) School.SetActive(false);
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
                ShowDialogue("Marc", "Grabe kayo sa akin. Nagre-reflect lang yung tao, 'di ba? Visualizing the success!");
                break;
            case 7:
                HideAllPortraits();
                if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(true);
                ShowDialogue("Darlene", "Guys, tama na 'yan. At least he tried, 'di ba? Anyway, check niyo muna gamit niyo. Ayokong bumalik at umakyat ng tatlong palapag dahil lang may nakalimutan kayong charger o payong.");
                break;
            case 8:
                HideAllPortraits();
                if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(true);
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
                ShowDialogue("Kuh", "True. Pero parang... ang bilis dumilim today? Kanina lang ang liwanag pa.");
                break;
            case 19:
                StartCoroutine(GlitchTransition());
                break;
            case 20:
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                ShowDialogue("Kuh", "...Wait. Narinig mo 'yun?");
                break;
            case 21:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                ShowDialogue("Cristel", "Ang alin?");
                break;
            case 22:
                // Added hallway fade in as requested
                if (backgroundHallway != null) StartCoroutine(FadeInObject(backgroundHallway, 2f));
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                ShowDialogue("Kuh", "Yung bell. Parang... iba yung tunog. Like a broken record. Or parang galing sa ilalim ng tubig na ewan.");
                break;
            case 23:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
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
                if (clockObject != null) StartCoroutine(FadeInObject(clockObject, 2f));
                ShowDialogue("", "A sudden chill runs down the corridor. The ticking of a clock becomes rhythmic and heavy.");
                break;
            case 25:
                if (sfxSource != null && intercomSound != null) sfxSource.PlayOneShot(intercomSound);
                ShowDialogue("Intercom", "The Angel of the Lord...");
                break;
            case 26:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                ShowDialogue("Cristel", "Wait, ang aga naman yata para sa prayer. 5:15 pa lang, 'di ba? Diba 6 PM dapat 'yun?");
                break;
            case 27:
                // Fixed: Clock must not be shown after this point
                if (clockObject != null) clockObject.SetActive(false);
                if (backgroundHallway != null) StartCoroutine(FadeInObject(backgroundHallway, 2f));
                if (shadowsObject != null) shadowsObject.SetActive(true);
                StartCoroutine(FlashEffect());
                ShowDialogue("", "The screen turns a deep, unnatural crimson. The shadows of the lockers stretch across the floor like long, dark fingers.");
                break;
            case 28:
                if (sfxSource != null) sfxSource.Stop();
                if (sfxSource != null && heartbeatSound != null) sfxSource.PlayOneShot(heartbeatSound);
                ShowDialogue("SYSTEM", "The clock shudders. Time stops. 5:17 PM.");
                break;
            case 29:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                ShowDialogue("Cristel", "...5:17? Pero 5:13 lang kanina sa phone ko. Lowbat ba 'to?");
                break;
            case 30:
                HideAllPortraits();
                if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(true);
                ShowDialogue("Kuh", "Cristel? Bakit biglang tumahimik? Nasaan na yung ibang mga estudyante? Kanina lang ang ingay dito...");
                break;
            case 31:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                ShowDialogue("Cristel", "Kuh? Stay close. Hindi ko na... hindi ko na makita yung dulo ng hallway. Parang may fog na ewan.");
                break;
            case 32:
                ShowDialogue("", "And then, the silence isn't just quiet. It is heavy, pressing against their ears. As if the school itself has stopped breathing.");
                break;
            case 33:
                HideAllPortraits();
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                ShowDialogue("Cristel", "Kuh, tara sa baba... baka andun sila sa lobby. Bilisan natin.");
                break;
            case 34:
                ShowDialogue("", "The darkness swallows the hallway as we move toward the stairs. Every step feels like we are sinking deeper into a cold, unfamiliar world.");
               
                break;
            case 35:
                HideAllPortraits();
                ShowDialogue("", "As the darkness pulls us in, my mind drifts back... back to when things were simpler.");
                break;
            case 36:
                StartCoroutine(FlashbackTransition());
                break;

            // --- FLASHBACK: HOW THEY MET ---
            case 37:
                HideAllPortraits();
                if (backgroundHallway != null) backgroundHallway.SetActive(true);
                if (backgroundCanteen != null) backgroundCanteen.SetActive(false);
                if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(true);
                ShowDialogue("Marc", "Tetel! Akalain mo 'yun, dito rin pala bagsak mo? Small world talaga!");
                break;
            case 38:
                HideAllPortraits();
                if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Hoy, Marc! Classmates na tayo since High School, pati ba naman sa college susundan mo ako? Akala ko ba sa Manila ka mag-aaral?");
                break;
            case 39:
                HideAllPortraits();
                if (portraitMarc != null) portraitMarc.SetActive(true);
                ShowDialogue("Marc", "Transfer na ako, 'no. Masyadong magulo dun. Tsaka siyempre, namiss kita as a classmate. Wala akong ma-bully dun eh.");
                break;
            case 40:
                HideAllPortraits();
                ShowDialogue("", "Marc and I have been friends since our high school days. He's always been the 'presko' one, but college was a fresh start for both of us.");
                break;
            case 41:
                HideAllPortraits();
                if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(true);
                ShowDialogue("Darlene", "Excuse me? Are you guys in the Psychology block too? Kanina ko pa kasi hinahanap yung Room 302.");
                break;
            case 42:
                HideAllPortraits();
                if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Ah, oo! Psychology rin kami. I'm Cristel, and this is Marc. Sabay ka na sa amin?");
                break;
            case 43:
                HideAllPortraits();
                if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(true);
                ShowDialogue("Darlene", "I'm Darlene. Thank God! Akala ko maliligaw na ako forever dito. Ang laki pala ng San Sebastian.");
                break;
            case 44:
                HideAllPortraits();
                if (backgroundHallway != null) backgroundHallway.SetActive(false);
                if (backgroundCanteen != null) backgroundCanteen.SetActive(true);
                ShowDialogue("", "A few weeks later, our duo became a trio. Darlene and I became inseparable during long lecture hours.");
                break;
            case 45:
                HideAllPortraits();
                if (portraitDarlene != null) portraitDarlene.SetActive(true);
                ShowDialogue("Darlene", "Guys, wait lang! May mga kakilala ako na dito rin nag-college. Introduce ko kayo, sa Canteen daw sila kakain.");
                break;
            case 46:
                HideAllPortraits();
                if (portraitRaven != null) portraitRaven.SetActive(true);
                ShowDialogue("Raven", "Darlene. You're 3 minutes and 14 seconds late. Standard procedure dictates we should have started eating.");
                break;
            case 47:
                HideAllPortraits();
                if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Grabe ka naman, Raven! Hayaan mo na, andito na siya oh! Huy Darlene! Namiss kita!");
                break;
            case 48:
                HideAllPortraits();
                if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(true);
                ShowDialogue("Darlene", "Cristel, Marc... this is Raven, classmate ko nung High School. And this is Kuh, schoolmate ko naman nung Senior High. Small world, 'di ba?");
                break;
            case 49:
                HideAllPortraits();
                if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(true);
                ShowDialogue("Marc", "Nice! More people to ignore Raven's logic lectures. Welcome to the group!");
                break;
            case 50:
                HideAllPortraits();
                if (backgroundCanteen != null) backgroundCanteen.SetActive(false);
                if (backgroundUncleJohns != null) backgroundUncleJohns.SetActive(true);
                ShowDialogue("", "And just like that, the pieces fell into place. We started spending every lunch break together.");
                break;
            case 51:
                HideAllPortraits();
                if (portraitKuh != null) portraitKuh.SetActive(true);
                ShowDialogue("Kuh", "Dali! Uncle John's tayo! Sabi nila masarap yung fried chicken dun, tsaka dun tayo mag-tambay habang mainit pa sa labas.");
                break;
            case 52:
                HideAllPortraits();
                if (portraitRaven != null) portraitRaven.SetActive(true);
                ShowDialogue("Raven", "Statistically, the chicken there is consistent. I agree with this decision.");
                break;
            case 53:
                HideAllPortraits();
                if (portraitCristel != null) portraitCristel.SetActive(true);
                ShowDialogue("Cristel", "Tignan mo 'tong mga 'to. Pagkain talaga nagpapasundo sa inyo. Tara na nga.");
                break;
            case 54:
                HideAllPortraits();
                ShowDialogue("", "At Uncle John's, amidst the smell of fried chicken and cheap air conditioning, we became more than just classmates.");
                break;
            case 55:
                HideAllPortraits();
                ShowDialogue("", "We became a family. A group of five against the world.");
                break;

            // --- RETURNING TO REALITY ---
            case 56:
                if (fadeOut != null) fadeOut.SetActive(false);
                StartCoroutine(GlitchTransition()); // Return to reality glitch
                break;
            case 57:
                HideAllPortraits();
                if (backgroundUncleJohns != null) backgroundUncleJohns.SetActive(false);
                if (backgroundHallway != null) backgroundHallway.SetActive(true);
                if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(true);
                ShowDialogue("Cristel", "...But that was then. Now, I can't even find where 'here' is.");
                break;
            case 58:
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

    IEnumerator FlashbackTransition()
    {
        if (nextButton != null) nextButton.SetActive(false);
        if (sfxSource != null && flashSound != null) sfxSource.PlayOneShot(flashSound);
        if (flashOverlay != null) flashOverlay.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        if (bgmSource != null && flashbackBGM != null)
        {
            bgmSource.clip = flashbackBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        if (fadeOut != null) fadeOut.SetActive(false);
        if (flashOverlay != null) flashOverlay.SetActive(false);
        OnNextClick();
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
        if (fadeOut != null) fadeOut.SetActive(false);
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

    IEnumerator FadeInObject(GameObject obj, float duration = 1.5f)
    {
        if (obj == null) yield break;

        // Using CanvasGroup for UI or SpriteRenderer alpha for sprites
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        cg.alpha = 0;
        obj.SetActive(true);

        float elapsed = 0;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;
    }

    IEnumerator FadeOutObject(GameObject obj)
    {
        if (obj != null) obj.SetActive(false);
        yield return null;
    }

    IEnumerator ZoomInObject(GameObject obj, float targetScale, float duration)
    {
        if (obj == null) yield break;
        Vector3 initialScale = obj.transform.localScale;
        Vector3 finalScale = initialScale * targetScale;
        float elapsed = 0;
        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = finalScale;
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
        if (portraitMarc_Laugh != null) portraitMarc_Laugh.SetActive(false);
        if (portraitCristel_Worried != null) portraitCristel_Worried.SetActive(false);
        if (portraitKuh_Scared != null) portraitKuh_Scared.SetActive(false);
        if (portraitDarlene_Smile != null) portraitDarlene_Smile.SetActive(false);
    }
}