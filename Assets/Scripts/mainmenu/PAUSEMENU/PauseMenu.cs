using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

/// <summary>
/// A reusable, robust mid-game pause controller with dynamic play/pause icon toggles.
/// Attach this script to a manager object inside your UI Canvas in every scene.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    // Global static flag to block dialogue progression when a pause screen is overlayed
    public static bool IsPaused = false;

    [System.Serializable]
    public class PauseSaveData
    {
        public string sceneName;
        public string saveDate;
        public int darleneTrust;
        public int cristelTrust;
        public int marcChances;
        public int marcTrust;
        public int kuhTrust;
        public int ravenTrust;
    }

    [Header("HUD Trigger Button")]
    [Tooltip("Assign the Pause Button located in the top-right of your screen.")]
    public Button hudPauseButton;

    [Header("HUD Icon Customization")]
    [Tooltip("The sprite displayed during active gameplay (Clicking this will pause).")]
    public Sprite pauseIcon;
    [Tooltip("The sprite displayed when the game is paused (Clicking this will resume).")]
    public Sprite playIcon;

    [Header("Pause Root Interfaces")]
    [Tooltip("The master background overlay holding the other panels as children. (DO NOT ASSIGN THE MASTER CANVAS HERE!)")]
    public GameObject pauseOverlayPanel;
    public GameObject pauseHomeSubPanel;     // Home panel with Resume/Save/Settings/Quit buttons
    public GameObject pauseSaveLoadSubPanel; // Sub-panel containing save slot interfaces
    public GameObject pauseSettingsSubPanel; // Sub-panel containing configuration sliders

    [Header("Navigation Buttons")]
    public Button resumeButton;
    public Button openSaveButton;
    public Button openLoadButton;
    public Button openSettingsButton;
    public Button quitToMainMenuButton;

    [Header("Sub-Panel Back Buttons")]
    public Button saveLoadBackButton;
    public Button settingsBackButton;

    [Header("Save/Load Slot UI Components")]
    public Button[] saveSlots;
    public TMP_Text[] saveSlotTexts;
    private bool isSaveMode = false; // If true, clicking slot saves progress. Otherwise, loads.

    [Header("Preferences (Settings) Controls")]
    public Slider textSpeedSlider;
    public Slider bgmVolumeSlider;
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
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            Debug.LogError("<color=red>PAUSE MENU ERROR:</color> No <b>EventSystem</b> found in your active scene hierarchy! Your mouse clicks and UI button interactions will not register without one. Right-click your Hierarchy and choose <b>UI -> Event System</b> immediately.");
        }
    }

    /// <summary>
    /// Binds click listeners programmatically to bypass manual Inspector events.
    /// </summary>
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

    /// <summary>
    /// Programmatically binds sliders so preferences write dynamically when dragging.
    /// </summary>
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

    /// <summary>
    /// Toggles pause state when clicking the top-right HUD button directly.
    /// </summary>
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

        // Toggle the HUD button image to show the Play icon (meaning "click to play/resume")
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

        // Retain standard VN cursor state (usually visible but free)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Toggle the HUD button image back to the Pause icon (meaning "click to pause")
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

    private void DeactivateSubPanels()
    {
        if (pauseHomeSubPanel != null) pauseHomeSubPanel.SetActive(false);
        if (pauseSaveLoadSubPanel != null) pauseSaveLoadSubPanel.SetActive(false);
        if (pauseSettingsSubPanel != null) pauseSettingsSubPanel.SetActive(false);
    }

    // ==========================================
    // MID-GAME PROGRESS RECORDING SYSTEM
    // ==========================================

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
                    saveSlotTexts[i].text = $"Slot {i + 1}\n<size=12>{data.sceneName}\n{data.saveDate}</size>";
                }
            }
            else
            {
                if (saveSlotTexts[i] != null)
                {
                    saveSlotTexts[i].text = $"Slot {i + 1}\n<size=12>Empty Slot</size>";
                }
            }
        }
    }

    private void OnSaveSlotClicked(int slotIndex)
    {
        string key = "SaveSlot_" + slotIndex;

        if (isSaveMode)
        {
            // Capture exact running state details
            PauseSaveData data = new PauseSaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
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

            Debug.Log($"<color=green>Pause Save:</color> Progress saved in Slot {slotIndex + 1}.");
            UpdateSaveSlotLabels();
        }
        else
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                PauseSaveData data = JsonUtility.FromJson<PauseSaveData>(json);

                // Restore exact stats back into the active PlayerPrefs before reloading the target scene
                PlayerPrefs.SetInt("DarleneTrust", data.darleneTrust);
                PlayerPrefs.SetInt("CristelTrust", data.cristelTrust);
                PlayerPrefs.SetInt("MarcChances", data.marcChances);
                PlayerPrefs.SetInt("MarcTrust", data.marcTrust);
                PlayerPrefs.SetInt("KuhTrust", data.kuhTrust);
                PlayerPrefs.SetInt("RavenTrust", data.ravenTrust);
                PlayerPrefs.Save();

                // Instantly clean up pausing flags and time systems before loading new scene
                ResumeGame();
                SceneManager.LoadScene(data.sceneName);
                Debug.Log($"<color=cyan>Pause Load:</color> Restored progress from Slot {slotIndex + 1}.");
            }
            else
            {
                Debug.LogWarning($"Pause Load Warning: Slot {slotIndex + 1} is empty!");
            }
        }
    }

    // ==========================================
    // SYSTEM PREFERENCES (SHARED VALUE KEYS)
    // ==========================================

    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("TextSpeed", textSpeedSlider != null ? textSpeedSlider.value : 0.02f);
        PlayerPrefs.SetFloat("BgmVol", bgmVolumeSlider != null ? bgmVolumeSlider.value : 1.0f);
        PlayerPrefs.SetFloat("SfxVol", sfxVolumeSlider != null ? sfxVolumeSlider.value : 1.0f);
        PlayerPrefs.Save();
    }

    private void LoadPreferences()
    {
        float textSpeed = PlayerPrefs.GetFloat("TextSpeed", 0.02f);
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

        if (bgmVolumeSlider != null) masterAudioMixer.SetFloat("BgmVolume", Mathf.Log10(bgmVolumeSlider.value) * 20f);
        if (sfxVolumeSlider != null) masterAudioMixer.SetFloat("SfxVolume", Mathf.Log10(sfxVolumeSlider.value) * 20f);
    }

    public void QuitToMainMenu()
    {
        // Safely unpause system before changing context
        ResumeGame();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}