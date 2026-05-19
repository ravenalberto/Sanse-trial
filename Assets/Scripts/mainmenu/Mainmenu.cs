using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

/// <summary>
/// A clean, serializable data structure representing a VN Save State.
/// This gets converted to JSON and stored directly in PlayerPrefs.
/// </summary>
[System.Serializable]
public class SaveSlotData
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

public class Mainmenu : MonoBehaviour
{
    [Header("Main Menu Navigation Buttons")]
    [Tooltip("Keep these buttons placed inside the Main Menu Panel!")]
    public Button playButton;
    public Button loadButton;
    public Button preferencesButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Sub-Panel Back Buttons")]
    [Tooltip("Back buttons located inside each sub-panel to return to the Main Menu.")]
    public Button saveLoadBackButton;
    public Button preferencesBackButton;
    public Button creditsBackButton;

    [Header("Menu Screen Panels")]
    [Tooltip("The main container panel holding the Start, Load, Settings, and Quit buttons.")]
    public GameObject mainMenuPanel;
    [Tooltip("The container panel holding the save slots and their texts.")]
    public GameObject saveLoadPanel;
    [Tooltip("The container panel holding the settings sliders and audio toggles.")]
    public GameObject preferencesPanel;
    [Tooltip("The container panel holding the game credits.")]
    public GameObject creditsPanel;

    [Header("Save/Load Slot UI Components")]
    [Tooltip("These slot buttons must be nested inside the Save/Load Panel.")]
    public Button[] saveSlots;
    public TMP_Text[] saveSlotTexts;
    private bool isSaveMode = false; // Controls if the panel is saving or loading progress

    [Header("Preferences (Settings) Controls")]
    [Tooltip("These sliders must be nested inside the Preferences Panel so they only display when Settings is open.")]
    public Slider textSpeedSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioMixer masterAudioMixer; // Optional: Link your AudioMixer asset here

    void Start()
    {
        // Unlock and make the mouse cursor visible for the menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Auto-bind click listeners to the Inspector buttons
        BindMenuButtons();

        // Enforce the strict UI layout state immediately on start
        ShowMainMenuOnly();

        // Load and apply the player's saved settings (volume, text speed)
        LoadPreferences();

        // Initialize the visual state of the save slots
        UpdateSaveSlotLabels();
    }

    /// <summary>
    /// Binds click listeners to UI Buttons automatically if assigned in the Inspector.
    /// </summary>
    private void BindMenuButtons()
    {
        // Main menu navigation buttons
        if (playButton != null) playButton.onClick.AddListener(OnNewGameClicked);
        if (loadButton != null) loadButton.onClick.AddListener(OpenLoadPanel);
        if (preferencesButton != null) preferencesButton.onClick.AddListener(OpenPreferencesPanel);
        if (creditsButton != null) creditsButton.onClick.AddListener(OpenCreditsPanel);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGameApplication);

        // Sub-panel Back buttons to return home
        if (saveLoadBackButton != null) saveLoadBackButton.onClick.AddListener(ShowMainMenuOnly);
        if (preferencesBackButton != null) preferencesBackButton.onClick.AddListener(ShowMainMenuOnly);
        if (creditsBackButton != null) creditsBackButton.onClick.AddListener(ShowMainMenuOnly);
    }

    // ==========================================
    // EXPLICIT PANEL CONTROLS & TRANSITIONS
    // ==========================================

    /// <summary>
    /// Deactivates all sub-panels and cleanly shows only the main menu navigation panel with its buttons.
    /// </summary>
    public void ShowMainMenuOnly()
    {
        DeactivateAllSubPanels();

        // ONLY enable the main menu panel (holding Start, Load, Settings, Quit buttons)
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Opens the Save slot interface and hides everything else.
    /// </summary>
    public void OpenSavePanel()
    {
        isSaveMode = true;
        DeactivateAllSubPanels();

        // ONLY enable the save/load slots panel
        if (saveLoadPanel != null) saveLoadPanel.SetActive(true);
        UpdateSaveSlotLabels();
    }

    /// <summary>
    /// Opens the Load slot interface and hides everything else.
    /// </summary>
    public void OpenLoadPanel()
    {
        isSaveMode = false;
        DeactivateAllSubPanels();

        // ONLY enable the save/load slots panel
        if (saveLoadPanel != null) saveLoadPanel.SetActive(true);
        UpdateSaveSlotLabels();
    }

    /// <summary>
    /// Opens the Preferences panel (showing sliders) and hides everything else.
    /// </summary>
    public void OpenPreferencesPanel()
    {
        DeactivateAllSubPanels();

        // ONLY enable the setting panel (holding your sliders)
        if (preferencesPanel != null) preferencesPanel.SetActive(true);
        LoadPreferences(); // Pull and apply the latest values from memory
    }

    /// <summary>
    /// Opens the Credits panel and hides everything else.
    /// </summary>
    public void OpenCreditsPanel()
    {
        DeactivateAllSubPanels();

        // ONLY enable the credits container panel
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Strictly deactivates all panels to prevent overlapping elements, buttons, and sliders.
    /// </summary>
    private void DeactivateAllSubPanels()
    {
        // Deactivates panels along with all their nested child objects (buttons, sliders, etc.)
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (preferencesPanel != null) preferencesPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // ==========================================
    // STORY START & SAVE / LOAD LOGIC
    // ==========================================

    public void OnNewGameClicked()
    {
        // Reset dynamic transition and chapter flags
        PlayerPrefs.SetInt("CameFromPacman", 0);
        PlayerPrefs.SetInt("CameFromBlockPuzzle", 0);

        // Reset trust scores and chances to Chapter 1 defaults
        PlayerPrefs.SetInt("DarleneTrust", 10);
        PlayerPrefs.SetInt("CristelTrust", 15);
        PlayerPrefs.SetInt("MarcChances", 3);
        PlayerPrefs.SetInt("MarcTrust", 10);
        PlayerPrefs.SetInt("KuhTrust", 10);
        PlayerPrefs.SetInt("RavenTrust", 10);
        PlayerPrefs.Save();

        // Loads Scene00 (the very beginning of the visual novel)
        SceneManager.LoadScene("Scene00");
    }

    private void UpdateSaveSlotLabels()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            string key = "SaveSlot_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                SaveSlotData data = JsonUtility.FromJson<SaveSlotData>(json);
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

    public void OnSaveSlotClicked(int slotIndex)
    {
        string key = "SaveSlot_" + slotIndex;

        if (isSaveMode)
        {
            // Gather active scene state to create a progress save file dynamically from PlayerPrefs session
            SaveSlotData data = new SaveSlotData
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

            Debug.Log($"<color=green>Save Success:</color> Progress saved in Slot {slotIndex + 1}.");
            UpdateSaveSlotLabels();
        }
        else
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                SaveSlotData data = JsonUtility.FromJson<SaveSlotData>(json);

                // Restore exact stats back into the active PlayerPrefs before reloading the target scene
                PlayerPrefs.SetInt("DarleneTrust", data.darleneTrust);
                PlayerPrefs.SetInt("CristelTrust", data.cristelTrust);
                PlayerPrefs.SetInt("MarcChances", data.marcChances);
                PlayerPrefs.SetInt("MarcChrust", data.marcTrust);
                PlayerPrefs.SetInt("KuhTrust", data.kuhTrust);
                PlayerPrefs.SetInt("RavenTrust", data.ravenTrust);
                PlayerPrefs.Save();

                // Load the exact visual novel scene associated with this slot
                SceneManager.LoadScene(data.sceneName);
                Debug.Log($"<color=cyan>Load Success:</color> Restored progress from Slot {slotIndex + 1} ({data.sceneName}).");
            }
            else
            {
                Debug.LogWarning($"Save Slot {slotIndex + 1} is empty!");
            }
        }
    }

    // ==========================================
    // SYSTEM PREFERENCES (VOLUME & CONTROLS)
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

        // Convert the linear slider values (0 to 1) into decibel values (-80dB to 20dB) for the Mixer
        if (bgmVolumeSlider != null) masterAudioMixer.SetFloat("BgmVolume", Mathf.Log10(bgmVolumeSlider.value) * 20f);
        if (sfxVolumeSlider != null) masterAudioMixer.SetFloat("SfxVolume", Mathf.Log10(sfxVolumeSlider.value) * 20f);
    }

    // ==========================================
    // APPLICATION EXIT
    // ==========================================

    public void QuitGameApplication()
    {
        Debug.Log("Exiting Visual Novel Application...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}