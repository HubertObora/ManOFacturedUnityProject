using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa obsługujące menu główne
/// </summary>
public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject mainLayout;
    [SerializeField] private GameObject settingsLayout;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private GameObject gearPrefab;
    [SerializeField] private Text gameVersion;

    private ConfirmationDialogSystem _confirmationDialogSystem;
    private InfoDialogSystem _informationDialogSystem;

    private void Awake() {
        QualitySettings.SetQualityLevel((int)SettingsManager.gameGraphics);
        _confirmationDialogSystem = GameObject.FindWithTag("ConfirmationDialog").GetComponent<ConfirmationDialogSystem>();
        _informationDialogSystem = GameObject.FindWithTag("InfoDialogSystem").GetComponent<InfoDialogSystem>();
        SettingsManager.LoadSettings();
    }

    private void Start() {
        gameVersion.text = GameController.GameVersion;
        PlayerPrefs.SetInt("IsGameContinued", 0);
    }

    /// <summary>
    /// Metoda, obsługująca naciskanie przycisku z autorami
    /// </summary>
    public void AuthorsButton() {
        _informationDialogSystem.ShowDialog("Authors: \nŻabka 1\nŻabka 2\nŻabka 3\nŻabka 4", TextAnchor.MiddleCenter);
    }

    /// <summary>
    /// Metoda, obsługująca naciskanie przycisku kontynuacji gry
    /// </summary>
    public void ContinueGameButton() {
        PlayerPrefs.SetInt("IsGameContinued", 1);

        if (GameSaver.GameSave != null) {
            SceneManager.LoadScene(GameSaver.GameSave.SceneId);    
        }
    }

    /// <summary>
    /// Matoda, obsługująca rozpoczęcie nowej gry
    /// </summary>
    public void NewGameButton() {
        if (GameSaver.IsGameSaveExists) {
            _confirmationDialogSystem.ShowDialog("Masz zapisaną grę, chcesz zacząć nową grę?", "Tak", "Nie", () => {
                SceneManager.LoadScene("Scenes/Level 1");
                GameController.CollectedGears = 0;
            });
        } else {
            SceneManager.LoadScene("Scenes/Level 1");
        }
    }

    /// <summary>
    /// Metoda, obsługująca naciskanie na przycisk wyświetlania ustawień
    /// </summary>
    public void SettingsButton() {
        mainLayout.SetActive(false);
        settingsLayout.SetActive(true);
    }
    
    /// <summary>
    /// Metoda, obsługująca naciskanie na przycisk wyjścia z gry
    /// </summary>
    public void ExitButton() {
        if (_confirmationDialogSystem != null) {
            _confirmationDialogSystem.ShowDialog("Zamknąć grę?", "Tak", "Nie", Application.Quit);   
        }
    }
}
