using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Klasa, odpowiadająca za wyświetlanie informacji o wartości scrollbara przy naciskaniu na przesuwak
/// </summary>
public class DisplayValueOnScrollbar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private Text value;

    private Scrollbar _scrollbar;

    private void Start() {
        _scrollbar = gameObject.GetComponent<Scrollbar>();

        value.text = GetValueInString(_scrollbar.value);
        
        _scrollbar.onValueChanged.AddListener(e => {
            value.text = GetValueInString(e);
        });
    }

    public void OnPointerDown(PointerEventData eventData) {
        value.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData) {
        value.gameObject.SetActive(false);
    }

    private string GetValueInString(float val) {
        return ((int)(val * 100)).ToString();
    }
}
