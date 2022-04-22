using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa, pozwalająca na tworzenie okna dialogowego konfirmacji jakiegoś działania
/// </summary>
public class ConfirmationDialogSystem : MonoBehaviour {
    [SerializeField] private GameObject confirmationDialog;
    
    /// <summary>
    /// Wyświetla dialogowe okno konfirmacji dla przekazanego działania
    /// </summary>
    /// <param name="title">Nazwa okna konfirmacji</param>
    /// <param name="positiveButtonText">Napis na przycisku potwierdzającym działanie</param>
    /// <param name="negativeButtonText">Napis na przycisku niepotwierdzającym działanie</param>
    /// <param name="action">Działanie, które musi być wykonane przy potwierdzeniu</param>
    public void ShowDialog(string title, string positiveButtonText, string negativeButtonText, Action action) {
        var dialogInstance = Instantiate(confirmationDialog, transform);
        var dialog = dialogInstance.GetComponent<ConfirmationDialog>();

        if (dialog != null) {
            dialog.title.text = title;
            dialog.positiveButton.GetComponentInChildren<Text>().text = positiveButtonText;
            dialog.negativeButton.GetComponentInChildren<Text>().text = negativeButtonText;
            
            dialog.positiveButton.onClick.AddListener(() => {
                action();
                Destroy(dialogInstance);
            });
            dialog.negativeButton.onClick.AddListener(() => {
                Destroy(dialogInstance);
            });
        }
    }
}
