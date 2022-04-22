using Skrypty.Enums;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Odpowiada za wyświetlanie danych w HUD
/// </summary>
public class HudController : MonoBehaviour {
  [SerializeField] private GameObject player;
  [SerializeField] private Image hpBar;
  [SerializeField] private Image ammoBar;
  [SerializeField] private Text playTime;
  [SerializeField] private Text collectedGears;

  private CharacterController2D playerController;
  private float hpBarWidthRate;
  private float ammoBarWidthRate;
  private RectTransform _hpRectTransform;
  private RectTransform _ammoRectTransform;

  private void Start() {
    _hpRectTransform = hpBar.gameObject.GetComponent<RectTransform>();
    _ammoRectTransform = ammoBar.gameObject.GetComponent<RectTransform>();
    playerController = player.GetComponent<CharacterController2D>();

    if (_hpRectTransform != null) {
      hpBarWidthRate = _hpRectTransform.rect.width / CharacterController2D.MaxHealth; 
    }

    if (_ammoRectTransform != null) {
      ammoBarWidthRate = _ammoRectTransform.rect.width / CharacterController2D.MaxAmmo;
    }

    if (playerController != null) {
      playerController.OnDamageTaken += ChangeHpBar;
      playerController.OnItemCollected += ChangeCollectedItems;
      playerController.OnShoot += ChangeAmmoBar;
      GameController.OnTakeGears += amount => {
        ChangeCollectedItems(PickUpItemsEnum.Gears, -amount);
      };
      InitializeAmmoBarValues(); 
    }

    collectedGears.text = GameController.CollectedGears.ToString();
    playTime.text = GameController.LevelPlayTime.ToString(@"mm\:ss");
  }

  /// <summary>
  /// Inicjalizuje wartość pasku amunicji
  /// </summary>
  private void InitializeAmmoBarValues() {
    var rect = _ammoRectTransform.rect;
    var newWidth = (CharacterController2D.Ammo * ammoBarWidthRate) >= 0 ? (CharacterController2D.Ammo * ammoBarWidthRate) : 0;
    _ammoRectTransform.sizeDelta = new Vector2(newWidth, rect.height);
  }

  /// <summary>
  /// Zmienia rozmiar pasku życia
  /// </summary>
  /// <param name="value">Wartość, na którą musi być zmieniony pasek życia</param>
  private void ChangeHpBar(float value) {
    if (_hpRectTransform != null) {
      var rect = _hpRectTransform.rect;
      var newWidth = rect.width - (value * hpBarWidthRate) >= 0 ? rect.width - (value * hpBarWidthRate) : 0;
      _hpRectTransform.sizeDelta = new Vector2(newWidth, rect.height);
    }
  }

  /// <summary>
  /// Zmienia rozmiar pasku amunicji
  /// </summary>
  /// <param name="value">Wartość, na którą musi być zmieniony pasek amunicji</param>
  private void ChangeAmmoBar(int value) {
    if (_ammoRectTransform != null) {
      if (value != 0) {
        var rect = _ammoRectTransform.rect;

        var newWidth = 0f;
        var valueToAdd = value * ammoBarWidthRate;
        if (value > 0) {
          newWidth = rect.width + valueToAdd <= 300 ? rect.width + valueToAdd : 300;  
        } else {
          newWidth = rect.width + valueToAdd >= 0 ? rect.width + valueToAdd : 0;
        }
      
        _ammoRectTransform.sizeDelta = new Vector2(newWidth, rect.height);
      }
    }
  }

  /// <summary>
  /// Obsługuje wyświetlanie przy zbieraniu podejmowanych obiektów
  /// </summary>
  /// <param name="type">Typ obiektu, który gracz podjął</param>
  /// <param name="value">Ilośc obiektów, które gracz podjął</param>
  private void ChangeCollectedItems(PickUpItemsEnum type, int value) {
    switch (type) {
      case PickUpItemsEnum.Gears:
        collectedGears.text = (int.Parse(collectedGears.text) + value).ToString();
        break;
      case PickUpItemsEnum.Battery:
        if (playerController != null) {
          ChangeAmmoBar(value); 
        }
        break;
    }
  }
}
