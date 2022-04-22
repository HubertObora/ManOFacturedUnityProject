using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa, odpowiadająca za obsługę menu w grze
/// </summary>
public class InGameMenu : MonoBehaviour {
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject mainLayout;
    [SerializeField] private GameObject settingsLayout;

    private ConfirmationDialogSystem _confirmationDialogSystem;

    private void Start() {
        _confirmationDialogSystem = GameObject.FindWithTag("ConfirmationDialog").GetComponent<ConfirmationDialogSystem>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (inGameMenu.activeSelf) {
                ContinueGame();
                GameController.IsGamePaused = false;
            } else {
                PauseGame();
                GameController.IsGamePaused = true;
            }
        }
    }

    /// <summary>
    /// Stawi grę na pauzę i wyświetla menu
    /// </summary>
    private void PauseGame() {
        inGameMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Kontynuuje grę i chowa menu
    /// </summary>
    private void ContinueGame() {
        inGameMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Metoda obsługująca naciskanie przycisku kontynuacji gry
    /// </summary>
    public void ContinueButton() {
        if (inGameMenu.activeSelf) {
            ContinueGame();
        }
    }
    
    /// <summary>
    /// Metoda obsługująca naciskanie przycisku ustawien. Chowa menu i wyświetla ustawienia
    /// </summary>
    public void SettingsButton() {
        mainLayout.SetActive(false);
        settingsLayout.SetActive(true);
    }

    
    /// <summary>
    /// Metoda obsługująca naciskanie przycisku powrotu do głównego menu
    /// </summary>
    public void ExitButton() {
        if (_confirmationDialogSystem != null) {
            _confirmationDialogSystem.ShowDialog("Wrócić do głównego menu?", "Tak", "Nie", () => {
                GameSaver.SaveGame();
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            });
        }
    }
}
