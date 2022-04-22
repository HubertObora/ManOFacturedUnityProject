using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa, odpowiadająca za wyświetlanie dialogu z informacjami
/// </summary>
public class InfoDialogSystem : MonoBehaviour {
    [SerializeField] private GameObject infoDialog;

    private string[] _text;

    /// <summary>
    /// Wyświetla dialog z informacjami
    /// </summary>
    /// <param name="text">Łańcuch znaków, który musi być wyświetlony</param>
    /// <param name="textAnchor">Ustawienia pozycjonowania łańcuchu znaków w oknie dialogowym</param>
    public void ShowDialog(string text, TextAnchor textAnchor = TextAnchor.UpperLeft) {
        var dialogInstance = Instantiate(infoDialog, transform);
        var dialog = dialogInstance.GetComponent<InformationDialog>();

        if (dialog != null) {
            dialog.text.text = text;
            dialog.text.alignment = textAnchor;
            dialog.closeButton.onClick.AddListener(() => {
                Destroy(dialogInstance);
            });
        }
    }

    /// <summary>
    /// Wyświetla dialog z informacjami
    /// </summary>
    /// <param name="text">Lista łańcuchów znaków, które muszą być wyświetlone po kolei w oknie dialogowym</param>
    /// <param name="textAnchor">Ustawienia pozycjonowania łańcuchu znaków w oknie dialogowym</param>
    public void ShowDialog(string[] text, TextAnchor textAnchor = TextAnchor.UpperLeft) {
        var dialogInstance = Instantiate(infoDialog, transform);
        var dialog = dialogInstance.GetComponent<InformationDialog>();
        var i = 0;

        if (dialog != null) {
            dialog.nextButton.gameObject.SetActive(true);
            dialog.text.text = text[i];
            dialog.text.alignment = textAnchor;
            dialog.closeButton.onClick.AddListener(() => {
                Destroy(dialogInstance);
            });
            dialog.nextButton.onClick.AddListener(() => {
                ++i;

                if (i == text.Length) {
                    Destroy(dialogInstance);
                } else if (i == text.Length - 1) {
                    dialog.text.text = text[i];
                    dialog.nextButton.GetComponentInChildren<Text>().text = "X";
                } else {
                    dialog.text.text = text[i];
                }
            });
        }
    }
    
    private void Update() {
        // For tests only
        if (Input.GetKeyDown(KeyCode.I)) {
            ShowDialog("Przykład wyświetlenia dialogu z informacjami");
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            ShowDialog(new []{"Coś", "Nowe", "Info?", "Chyba tak"});
        }
    }
}
