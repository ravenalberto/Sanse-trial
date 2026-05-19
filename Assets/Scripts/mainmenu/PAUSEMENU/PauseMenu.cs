using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems; // Required for integrated click-raycast diagnostics
using TMPro;

/// <summary>
/// A reusable, robust mid-game pause controller with dynamic play/pause icon toggles.
/// Contains an integrated Raycast Debugger, a multi-slot Save/Load engine, and a 
/// new dynamic Tutorial overlay for gameplay direction mechanics.
/// Includes dynamic BGM and Audio Track preservation features upon continuing saved games.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    // Global static flag to block dialogue progression when a pause screen is overlayed
    public static bool IsPaused = false;

    // --- CRITICAL DATA VARIABLES: TRACKS ACTIVE SCENE PROGRESSION ---
    public static int ActiveLineIndex = 0;
    public static string ActiveDialogueText = "Starting game...";
    public static string ActiveMusicTrackName = ""; // Tracks the active BGM identifier globally for saves

    [System.Serializable]
    public class PauseSaveData
    {
        public string sceneName;
        public string saveDate;
        public int lineIndex;
        public string dialoguePreview;
        public string musicTrackName; // Preserves the active music state
        public int darleneTrust;
        public int cristelTrust;
        public int marcChances;
        public int marcTrust;
        public int kuhTrust;
        public int ravenTrust;
    }

    [Header("Diagnostic Settings")]
    [Tooltip("If true, clicking anywhere on the screen will print which UI element is blocking your mouse click.")]
    public bool enableRaycastDebugging = true;

    [Header("HUD Trigger Button")]
    [Tooltip("Assign the Pause Button located in the top-right of your screen.")]
    public Button hudPauseButton;

    [Header("HUD Icon Customization")]
    public Sprite pauseIcon;
    public Sprite playIcon;

    [Header("Pause Root Interfaces")]
    [Tooltip("The master background overlay holding the other panels as children. (DO NOT ASSIGN THE MASTER CANVAS HERE!)")]
    public GameObject pauseOverlayPanel;
    public GameObject pauseHomeSubPanel;     // Home panel with Resume/Save/Settings/Quit buttons
    public GameObject pauseSaveLoadSubPanel; // Sub-panel containing save slot interfaces
    public GameObject pauseSettingsSubPanel; // Sub-panel containing configuration sliders
    public GameObject pauseTutorialSubPanel; // New sub-panel containing direction mechanics

    [Header("Navigation Buttons")]
    public Button resumeButton;
    public Button openSaveButton;
    public Button openLoadButton;
    public Button openSettingsButton;
    public Button openTutorialButton;        // New button to open tutorial from home screen
    public Button quitToMainMenuButton;

    [Header("Sub-Panel Back Buttons")]
    public Button saveLoadBackButton;
    public Button settingsBackButton;
    public Button tutorialBackButton;        // New back button to return to pause home

    [Header("Save/Load Slot UI Components")]
    public Button[] saveSlots;
    public TMP_Text[] saveSlotTexts;
    private bool isSaveMode = false; // If true, clicking slot saves progress. Otherwise, loads.

    [Header("Preferences (Settings) Controls")]
    [Tooltip("Set your text speed slider's Min Value to 0 and Max Value to 1 in the Inspector.")]
    public Slider textSpeedSlider;
    [Tooltip("Set your volume sliders' Min Value to 0.0001 and Max Value to 1 in the Inspector.")]
    public Slider bgmVolumeSlider;
    [Tooltip("Set your volume sliders' Min Value to 0.0001 and Max Value to 1 in the Inspector.")]
    public Slider sfxVolumeSlider;
    public AudioMixer masterAudioMixer;

    [Header("Scene Configuration")]
    [Tooltip("The exact name of your main menu scene.")]
    public string mainMenuSceneName = "Scene00";

    void Start()
    {
        IsPaused = false;

        // --- CRITICAL SAFETY DIAGNOSTIC: PREVENT BLUE SCREEN OF DEATH ---
        if (pauseOverlayPanel != null)
        {
            Canvas canvasCheck = pauseOverlayPanel.GetComponent<Canvas>();
            if (canvasCheck != null)
            {
                Debug.LogError("<color=red>PAUSE MENU SETUP ERROR:</color> You assigned your master <b>Canvas</b> GameObject to the <b>'Pause Overlay Panel'</b> slot! This turns off the entire UI on start, causing a solid blue screen. Please assign the child <b>'PausePanel'</b> object instead.", pauseOverlayPanel);
            }
            else
            {
                // Safely deactivate ONLY the overlay panel on start
                pauseOverlayPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("<color=yellow>PAUSE MENU WARNING:</color> 'Pause Overlay Panel' is not assigned in the Inspector. The pause screen will not be able to open.");
        }

        DeactivateSubPanels();

        // Initialize HUD Pause button to show the pause icon on start (as the game is running)
        ResetHUDButtonIcon();

        // Bind Action Listeners
        BindPauseButtons();
        BindSliderListeners();

        // Check for missing UI EventSystem
        if (EventSystem.current == null)
        {
            Debug.LogError("<color=red>PAUSE MENU ERROR:</color> No <b>EventSystem</b> found in your active scene hierarchy! Your mouse clicks and UI button interactions will not register without one. Right-click your Hierarchy and choose <b>UI -> Event System</b> immediately.");
        }
    }

    void Update()
    {
        // --- INTEGRATED CLICK BLOCK DIAGNOSTIC TOOL ---
        if (enableRaycastDebugging && Input.GetMouseButtonDown(0))
        {
            RunRaycastDiagnostic();
        }
    }

    /// <summary>
    /// GLOBAL HELPER: Translates the Text Speed slider (0.0 to 1.0) into actual dialogue delay seconds.
    /// </summary>
    public static float GetTextDelay()
    {
        float sliderValue = PlayerPrefs.GetFloat("TextSpeed", 0.5f);
        return Mathf.Lerp(0.08f, 0.002f, sliderValue);
    }

    private void RunRaycastDiagnostic()
    {
        if (EventSystem.current == null) return;
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            GameObject hitObject = results[0].gameObject;
            string path = hitObject.name;
            Transform t = hitObject.transform.parent;
            while (t != null) { path = t.name + "/" + path; t = t.parent; }
            Debug.Log($"<color=cyan>[Pause Diagnostic]:</color> Click intercepted by <b>{hitObject.name}</b> at path: <b>{path}</b>", hitObject);
        }
    }

    private void BindPauseButtons()
    {
        // HUD Trigger acts as a direct toggle between states
        if (hudPauseButton != null)
        {
            hudPauseButton.onClick.RemoveAllListeners();
            hudPauseButton.onClick.AddListener(TogglePauseState);
            Debug.Log("<color=green>PauseMenu Logic:</color> Successfully bound hudPauseButton.");
        }
        else
        {
            Debug.LogWarning("<color=yellow>PauseMenu Alert:</color> 'hudPauseButton' is empty in the Inspector!");
        }

        // Core Actions
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }
        if (openSaveButton != null)
        {
            openSaveButton.onClick.RemoveAllListeners();
            openSaveButton.onClick.AddListener(OpenSaveSubPanel);
        }
        if (openLoadButton != null)
        {
            openLoadButton.onClick.RemoveAllListeners();
            openLoadButton.onClick.AddListener(OpenLoadSubPanel);
        }
        if (openSettingsButton != null)
        {
            openSettingsButton.onClick.RemoveAllListeners();
            openSettingsButton.onClick.AddListener(OpenSettingsSubPanel);
        }
        if (openTutorialButton != null)
        {
            openTutorialButton.onClick.RemoveAllListeners();
            openTutorialButton.onClick.AddListener(OpenTutorialSubPanel);
        }
        if (quitToMainMenuButton != null)
        {
            quitToMainMenuButton.onClick.RemoveAllListeners();
            quitToMainMenuButton.onClick.AddListener(QuitToMainMenu);
        }

        // Back buttons
        if (saveLoadBackButton != null)
        {
            saveLoadBackButton.onClick.RemoveAllListeners();
            saveLoadBackButton.onClick.AddListener(ShowPauseHome);
        }
        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveAllListeners();
            settingsBackButton.onClick.AddListener(ShowPauseHome);
        }
        if (tutorialBackButton != null)
        {
            tutorialBackButton.onClick.RemoveAllListeners();
            tutorialBackButton.onClick.AddListener(ShowPauseHome);
        }

        // Save Slot triggers
        for (int i = 0; i < saveSlots.Length; i++)
        {
            int index = i; // Closure safety check
            if (saveSlots[i] != null)
            {
                saveSlots[index].onClick.RemoveAllListeners();
                saveSlots[index].onClick.AddListener(() => OnSaveSlotClicked(index));
            }
        }
    }

    private void BindSliderListeners()
    {
        if (textSpeedSlider != null)
        {
            textSpeedSlider.onValueChanged.RemoveAllListeners();
            textSpeedSlider.onValueChanged.AddListener(delegate { OnPreferenceChanged(); });
        }
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.RemoveAllListeners();
            bgmVolumeSlider.onValueChanged.AddListener(delegate { OnPreferenceChanged(); });
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(delegate { OnPreferenceChanged(); });
        }
    }

    // ==========================================
    // PORTAL AND OVERLAY TRANSITIONS
    // ==========================================

    public void TogglePauseState()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            TriggerPause();
        }
    }

    public void TriggerPause()
    {
        IsPaused = true;
        Time.timeScale = 0f; // Freeze game physics/time systems

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (hudPauseButton != null && hudPauseButton.image != null && playIcon != null)
        {
            hudPauseButton.image.sprite = playIcon;
        }

        if (pauseOverlayPanel != null) pauseOverlayPanel.SetActive(true);
        ShowPauseHome();
        Debug.Log("<color=cyan>Pause System:</color> Game Paused successfully.");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f; // Restore normal game speed

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ResetHUDButtonIcon();

        DeactivateSubPanels();
        if (pauseOverlayPanel != null) pauseOverlayPanel.SetActive(false);
        Debug.Log("<color=cyan>Pause System:</color> Game Resumed successfully.");
    }

    private void ResetHUDButtonIcon()
    {
        if (hudPauseButton != null && hudPauseButton.image != null && pauseIcon != null)
        {
            hudPauseButton.image.sprite = pauseIcon;
        }
    }

    private void ShowPauseHome()
    {
        DeactivateSubPanels();
        if (pauseHomeSubPanel != null) pauseHomeSubPanel.SetActive(true);
    }

    private void OpenSaveSubPanel()
    {
        isSaveMode = true;
        DeactivateSubPanels();
        if (pauseSaveLoadSubPanel != null) pauseSaveLoadSubPanel.SetActive(true);
        UpdateSaveSlotLabels();
    }

    private void OpenLoadSubPanel()
    {
        isSaveMode = false;
        DeactivateSubPanels();
        if (pauseSaveLoadSubPanel != null) pauseSaveLoadSubPanel.SetActive(true);
        UpdateSaveSlotLabels();
    }

    private void OpenSettingsSubPanel()
    {
        DeactivateSubPanels();
        if (pauseSettingsSubPanel != null) pauseSettingsSubPanel.SetActive(true);
        LoadPreferences();
    }

    private void OpenTutorialSubPanel()
    {
        DeactivateSubPanels();
        if (pauseTutorialSubPanel != null) pauseTutorialSubPanel.SetActive(true);
    }

    private void DeactivateSubPanels()
    {
        if (pauseHomeSubPanel != null) pauseHomeSubPanel.SetActive(false);
        if (pauseSaveLoadSubPanel != null) pauseSaveLoadSubPanel.SetActive(false);
        if (pauseSettingsSubPanel != null) pauseSettingsSubPanel.SetActive(false);
        if (pauseTutorialSubPanel != null) pauseTutorialSubPanel.SetActive(false);
    }

    // ==========================================
    // 💾 SAVE / LOAD PROGRESS CORE ENGINE
    // ==========================================

    private string GetChapterFriendlyName(string sceneName)
    {
        switch (sceneName)
        {
            case "Scene00":
            case "Scene00VN":
                return "Chapter 1: The Beginning";
            case "Scene01":
                return "Chapter 2: Raven's Eyes";
            case "Scene02":
            case "Scene02VN":
                return "Chapter 3: Kuh's Vision";
            case "Scene03":
                return "Chapter 3 Part 2: Comlab Escape";
            case "Scene04":
            case "Scene04VN":
            case "Scene04Events":
                return "Chapter 4: Broken Reality";
            case "Scene06":
                return "Chapter 5: Flashback";
            default:
                return "Custom Scene Progress";
        }
    }

    private void UpdateSaveSlotLabels()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            string key = "SaveSlot_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                PauseSaveData data = JsonUtility.FromJson<PauseSaveData>(json);
                if (saveSlotTexts[i] != null)
                {
                    string chapterName = GetChapterFriendlyName(data.sceneName);
                    string snippet = data.dialoguePreview;
                    if (string.IsNullOrEmpty(snippet)) snippet = "...";
                    if (snippet.Length > 28) snippet = snippet.Substring(0, 25) + "...";

                    saveSlotTexts[i].text = $"<b>Slot {i + 1}</b> - {chapterName}\n" +
                                           $"<size=11><color=#8AC2F9>{data.saveDate}</color></size>\n" +
                                           $"<size=12><i>\"{snippet}\"</i></size>";
                }
            }
            else
            {
                if (saveSlotTexts[i] != null)
                {
                    saveSlotTexts[i].text = $"<b>Slot {i + 1}</b>\n<size=12><color=#A0A0A0>Empty Slot</color></size>";
                }
            }
        }
    }

    private void OnSaveSlotClicked(int slotIndex)
    {
        string key = "SaveSlot_" + slotIndex;

        if (isSaveMode)
        {
            PauseSaveData data = new PauseSaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                lineIndex = ActiveLineIndex,
                dialoguePreview = ActiveDialogueText,
                musicTrackName = ActiveMusicTrackName, // Saves the active music track identifier
                darleneTrust = PlayerPrefs.GetInt("DarleneTrust", 10),
                cristelTrust = PlayerPrefs.GetInt("CristelTrust", 15),
                marcChances = PlayerPrefs.GetInt("MarcChances", 3),
                marcTrust = PlayerPrefs.GetInt("MarcTrust", 10),
                kuhTrust = PlayerPrefs.GetInt("KuhTrust", 10),
                ravenTrust = PlayerPrefs.GetInt("RavenTrust", 10)
            };

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

            Debug.Log($"<color=green>Pause Save:</color> Progress saved in Slot {slotIndex + 1}. Active Music Track: {data.musicTrackName}");
            UpdateSaveSlotLabels();
        }
        else
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                PauseSaveData data = JsonUtility.FromJson<PauseSaveData>(json);

                PlayerPrefs.SetInt("DarleneTrust", data.darleneTrust);
                PlayerPrefs.SetInt("CristelTrust", data.cristelTrust);
                PlayerPrefs.SetInt("MarcChances", data.marcChances);
                PlayerPrefs.SetInt("MarcTrust", data.marcTrust);
                PlayerPrefs.SetInt("KuhTrust", data.kuhTrust);
                PlayerPrefs.SetInt("RavenTrust", data.ravenTrust);

                PlayerPrefs.SetInt("SavedLineIndex", data.lineIndex);
                PlayerPrefs.SetString("SavedScene", data.sceneName);
                PlayerPrefs.SetString("SavedMusicTrack", data.musicTrackName); // Restores active track identifier for scene loading
                PlayerPrefs.Save();

                ResumeGame();
                SceneManager.LoadScene(data.sceneName);
                Debug.Log($"<color=cyan>Pause Load:</color> Loaded Slot {slotIndex + 1} ({data.sceneName}), resuming at Line {data.lineIndex} and Track {data.musicTrackName}.");
            }
        }
    }

    // ==========================================
    // ⚙️ SYSTEM SETTINGS / PREFERENCES
    // ==========================================

    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("TextSpeed", textSpeedSlider != null ? textSpeedSlider.value : 0.5f);
        PlayerPrefs.SetFloat("BgmVol", bgmVolumeSlider != null ? bgmVolumeSlider.value : 1.0f);
        PlayerPrefs.SetFloat("SfxVol", sfxVolumeSlider != null ? sfxVolumeSlider.value : 1.0f);
        PlayerPrefs.Save();
    }

    private void LoadPreferences()
    {
        float textSpeed = PlayerPrefs.GetFloat("TextSpeed", 0.5f);
        float bgmVol = PlayerPrefs.GetFloat("BgmVol", 1.0f);
        float sfxVol = PlayerPrefs.GetFloat("SfxVol", 1.0f);

        if (textSpeedSlider != null) textSpeedSlider.value = textSpeed;
        if (bgmVolumeSlider != null) bgmVolumeSlider.value = bgmVol;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVol;

        ApplyVolumes();
    }

    public void OnPreferenceChanged()
    {
        SavePreferences();
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (masterAudioMixer == null) return;

        float bgmValue = bgmVolumeSlider != null ? Mathf.Max(bgmVolumeSlider.value, 0.0001f) : 1f;
        float sfxValue = sfxVolumeSlider != null ? Mathf.Max(sfxVolumeSlider.value, 0.0001f) : 1f;

        masterAudioMixer.SetFloat("BgmVolume", Mathf.Log10(bgmValue) * 20f);
        masterAudioMixer.SetFloat("SfxVolume", Mathf.Log10(sfxValue) * 20f);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}