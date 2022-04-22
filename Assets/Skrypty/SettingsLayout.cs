using System;
using System.Linq;
using Skrypty.Enums;
using Skrypty.Extensions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa, obsługująca ustawienia
/// </summary>
public class SettingsLayout : MonoBehaviour {
    [SerializeField] private GameObject mainLayout;
    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private Dropdown graphics;
    [SerializeField] private Scrollbar effectsVolume;
    [SerializeField] private Scrollbar musicVolume;
    [SerializeField] private Dropdown resolution;
    [SerializeField] private Toggle isFullScreen;

    private ConfirmationDialogSystem _confirmationDialogSystem;

    private void Start() {
        _confirmationDialogSystem = GameObject.FindWithTag("ConfirmationDialog").GetComponent<ConfirmationDialogSystem>();
        SetResolutions();
    }

    private void OnEnable() {
        AssignValuesFromManager();
    }

    /// <summary>
    /// Metoda, obsługująca naciskanie na przycisk zastosowania zmian
    /// </summary>
    public void ApplyButton() {
        var (width, height) = resolution.options[resolution.value].text.Split('x');
        SettingsManager.gameResolution = new Resolution() {width = width, height = height, refreshRate = Screen.currentResolution.refreshRate};
        SettingsManager.gameIsFullScreen = isFullScreen.isOn;
        SettingsManager.gameGraphics = (GraphicsSettingsEnum)graphics.value;
        QualitySettings.SetQualityLevel((int)SettingsManager.gameGraphics);
        if (effectsAudioSource!= null) effectsAudioSource.volume = effectsVolume.value;
        SettingsManager.gameEffectsVolume = effectsVolume.value;
        if (musicAudioSource != null) musicAudioSource.volume = musicVolume.value;
        SettingsManager.gameMusicVolume = musicVolume.value;
        Screen.SetResolution(width, height, isFullScreen.isOn);
        gameObject.SetActive(false);
        mainLayout.SetActive(true);
        SettingsManager.SaveSettings();
    }
    
    /// <summary>
    /// Metoda, obsługująca naciskanie na przycisk odrzucenia zmian
    /// </summary>
    public void CancelButton() {
        if (CheckIfValuesChanged()) {
            if (_confirmationDialogSystem != null) {
                _confirmationDialogSystem.ShowDialog("Odrzuci� zmiany?", "Tak", "Nie",
                                                     () => {
                                                         CloseConfirmationDialog();
                                                         AssignValuesFromManager();
                                                     });
            }
        } else {
            CloseConfirmationDialog();
        }
    }

    /// <summary>
    /// Metoda, obsługująca naciskanie na przycisk przywrócenia domyślnych wartości
    /// </summary>
    public void ResetToDefaults() {
        if (_confirmationDialogSystem != null) {
            _confirmationDialogSystem.ShowDialog("Przywrócić domyślne?", "Tak", "Nie", () => {
                SettingsManager.gameResolution = SettingsManager.defaultResolution;
                SettingsManager.gameIsFullScreen = SettingsManager.defaultIsFullScreen;
                SettingsManager.gameGraphics = SettingsManager.defaultGraphics;
                SettingsManager.gameEffectsVolume = SettingsManager.defaultEffectsVolume;
                SettingsManager.gameMusicVolume = SettingsManager.defaultMusicVolume;
                Screen.SetResolution(SettingsManager.defaultResolution.width, SettingsManager.defaultResolution.height, SettingsManager.defaultIsFullScreen, SettingsManager.defaultResolution.refreshRate);
                gameObject.SetActive(false);
                mainLayout.SetActive(true);
            });
        }
    }

    /// <summary>
    /// Przypisuje wartości z managera ustawień
    /// </summary>
    private void AssignValuesFromManager() {
        SelectResolutionInDropbox(SettingsManager.gameResolution);
        isFullScreen.isOn = SettingsManager.gameIsFullScreen;
        graphics.value = (int)SettingsManager.gameGraphics;
        effectsAudioSource.volume = SettingsManager.gameEffectsVolume;
        effectsVolume.value = SettingsManager.gameEffectsVolume;
        musicAudioSource.volume = SettingsManager.gameMusicVolume;
        musicVolume.value = SettingsManager.gameMusicVolume;
    }

    /// <summary>
    /// Chowa dialog z konfirmacją
    /// </summary>
    private void CloseConfirmationDialog() {
        gameObject.SetActive(false);
        mainLayout.SetActive(true);
    }

    /// <summary>
    /// Weryfikuje, czy ustawienia zostały zmienione
    /// </summary>
    /// <returns>Wartość logiczną, reprezentującą czy zmienione były ustawienia</returns>
    private bool CheckIfValuesChanged() {
        if (!CheckIfResolutionsEqual(SettingsManager.gameResolution, Screen.currentResolution) ||
            SettingsManager.gameIsFullScreen != isFullScreen.isOn ||
            (int)SettingsManager.gameGraphics != graphics.value ||
            SettingsManager.gameEffectsVolume != effectsVolume.value ||
            SettingsManager.gameMusicVolume != musicVolume.value) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Porównuje dwa rozszerzenia ekranu
    /// </summary>
    /// <param name="resolution1">Pierwsze rozszerzenia do porównywania</param>
    /// <param name="resolution2">Drugie rozszerzenia do porównywania</param>
    /// <returns>Wartość logiczną, która reprezentuje, czy dwa rozszerzenia są równe</returns>
    private bool CheckIfResolutionsEqual(Resolution resolution1, Resolution resolution2) {
        if (resolution1.height == resolution2.height &&
            resolution1.width == resolution2.width &&
            resolution1.refreshRate == resolution2.refreshRate) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Pobiera wszystkie możliwe rozszerzenia monitora i dodaje ich do dropboxa
    /// </summary>
    private void SetResolutions() {
        foreach (var res in Screen.resolutions.Reverse()) {
            resolution.options.Add(new Dropdown.OptionData($"{res.width} x {res.height}"));
        }

        SelectResolutionInDropbox(SettingsManager.gameResolution);
    }

    /// <summary>
    /// Zwraca indeks rozszerzenia w liście możliwych rozszerzeń monitora
    /// </summary>
    /// <param name="res">Rozszerzenie, którego indeks musi być znaleziony</param>
    /// <returns>Indeks znalezionego rozszerzenia</returns>
    private int GetResolutionIndex(Resolution res) {
        return resolution.options.FindIndex(e => e.text == $"{res.width} x {res.height}");
    }

    /// <summary>
    /// Wybiera rozszerzenie w dropboxie
    /// </summary>
    /// <param name="res">Rozszerzenia, które bysi być wybrane</param>
    private void SelectResolutionInDropbox(Resolution res) {
        var resolutionIndex = GetResolutionIndex(res);

        if (resolutionIndex >= 0) {
            resolution.SetValueWithoutNotify(resolutionIndex);
        }
    }
}
