using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa, obsługujące selektory broni i wyposażenia
/// </summary>
public class SelectorsController : MonoBehaviour {
    [SerializeField] private GameObject[] lasers;
    [SerializeField] private Sprite[] weaponIcons;
    [SerializeField] private Sprite[] equipmentIcons;
    [SerializeField] private GameObject currentWeapon;
    [SerializeField] private GameObject possibleWeapons;
    [SerializeField] private GameObject currentEquipment;
    [SerializeField] private GameObject possibleEquipments;

    private Image currentWeaponIcon;
    private GameObject weaponExtendedArrowIcon;
    private Image currentEquipmentIcon;
    private GameObject equipmentExtendedArrowIcon;

    private bool isPossibleWeaponsShowed = false;
    private bool isPossibleEquipmentsShowed = false;

    private CharacterController2D _characterController;

    private List<Image> possibleWeaponsImages;
    private List<Image> possibleEquipmentsImages;

    void Start() {
        possibleEquipmentsImages = new List<Image>();
        possibleWeaponsImages = new List<Image>();
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();

        foreach (var component in currentWeapon.GetComponentsInChildren<Image>()) {
            if (component.gameObject.transform.parent.gameObject.name == "Background") {
                if (!_characterController.IsRedLaserPicked) {
                    var temp = component.color;
                    temp.a = 0.7f;
                    component.color = temp;
                }
                currentWeaponIcon = component;
            }

            if (component.gameObject.transform.parent.gameObject.name == "Arrow") {
                weaponExtendedArrowIcon = component.gameObject;
            }
        }

        foreach (var component in currentEquipment.GetComponentsInChildren<Image>()) {
            if (component.gameObject.transform.parent.gameObject.name == "Background") {
                currentEquipmentIcon = component;
            }
            
            if (component.gameObject.transform.parent.gameObject.name == "Arrow") {
                equipmentExtendedArrowIcon = component.gameObject;
            }
        }

        foreach (var component in possibleWeapons.GetComponentsInChildren<Image>()) {
            if (component.gameObject.transform.parent.gameObject.name == "Background") {
                if (component.sprite.name.ToLower().Contains("greenlaser")) {
                    if (!_characterController.IsGreenLaserPicked) {
                        var temp = component.color;
                        temp.a = 0.7f;
                        component.color = temp; 
                    }
                }
            }
        }

        foreach (var component in possibleEquipments.GetComponentsInChildren<Image>()) {
            if (component.gameObject.transform.parent.gameObject.name == "Background") {
                possibleEquipmentsImages.Add(component);

                if (component.sprite.name.ToLower().Contains("4")) {
                    if (!_characterController.IsJetpackPicked) {
                        var temp = component.color;
                        temp.a = 0.7f;
                        component.color = temp;
                    }
                }
            }
        }

        foreach (var image in possibleWeapons.GetComponentsInChildren<Image>()) {
            if (image.gameObject.transform.parent.gameObject.name == "Background") {
                possibleWeaponsImages.Add(image);
            }
        }

        _characterController.OnRedLaserPicked += () => {
            if (currentWeaponIcon.sprite.name.ToLower().Contains("redlaser")) {
                var temp = currentWeaponIcon.color;
                temp.a = 1f;
                currentWeaponIcon.color = temp;
            } else {
                foreach (var image in possibleWeaponsImages) {
                    if (image.sprite.name.ToLower().Contains("redlaser")) {
                        var temp = currentWeaponIcon.color;
                        temp.a = 1f;
                        image.color = temp;
                    }
                }
            }
        };
        
        _characterController.OnGreenLaserPicked += () => {
            if (currentWeaponIcon.sprite.name.ToLower().Contains("greenlaser")) {
                var temp = currentWeaponIcon.color;
                temp.a = 1f;
                currentWeaponIcon.color = temp;
            } else {
                foreach (var image in possibleWeaponsImages) {
                    if (image.sprite.name.ToLower().Contains("greenlaser")) {
                        var temp = currentWeaponIcon.color;
                        temp.a = 1f;
                        image.color = temp;
                    }
                }
            }
        };
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            SwitchPossibleWeapons();
        }
        
        if (Input.GetKeyDown(KeyCode.X)) {
            SwitchPossibleEquipments();
        }

        if (isPossibleEquipmentsShowed || isPossibleWeaponsShowed) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                Change(1);
            }   
        }
    }

    /// <summary>
    /// Metoda, odpowiadająca za zmianę broni i wyposażenia
    /// </summary>
    /// <param name="pos">Pozycja w liście możliwych broń lub wyposażenia, na które musi być zmieniona ikona aktualnej broni lub wyposażenia</param>
    private void Change(int pos) {
        if (isPossibleWeaponsShowed) {
            if (_characterController.IsGreenLaserPicked) {
                _characterController.ChangeLaser(lasers[pos]);
                ChangeWeaponsIcon(pos);
            } else {
                HidePossibleWeapons();
            }
        } else if (isPossibleEquipmentsShowed) {
            if (_characterController.canFly) {
                _characterController.EnableJump();
                ChangeEquipmentsIcon(1);
            } else {
                if (_characterController.IsJetpackPicked) {
                    _characterController.EnableJetPack();
                    ChangeEquipmentsIcon(1);   
                } else {
                    HidePossibleEquipments();
                }
            }
        }
    }

    /// <summary>
    /// Metoda, odpowiadająca za zmianę ikony broni
    /// </summary>
    /// <param name="pos">Pozycja w liście możliwych broń, na którą musi być zmieniona ikona aktualnej broni</param>
    private void ChangeWeaponsIcon(int pos) {
        (lasers[0], lasers[pos]) = (lasers[pos], lasers[0]);
        (weaponIcons[0], weaponIcons[pos]) = (weaponIcons[pos], weaponIcons[0]);
        currentWeaponIcon.sprite = weaponIcons[0];

        foreach (var image in possibleWeaponsImages) {
            if (image.sprite.name == weaponIcons[0].name) {
                image.sprite = weaponIcons[pos];
            }
        }
        
        HidePossibleWeapons();
    }

    /// <summary>
    /// Metoda, odpowiadająca za zmianę ikony wyposażenia
    /// </summary>
    /// <param name="pos">Pozycja w liście możliwych wyposażenia, na którą musi być zmieniona ikona aktualnego wyposażenia</param>
    private void ChangeEquipmentsIcon(int pos) {
        (equipmentIcons[0], equipmentIcons[pos]) = (equipmentIcons[pos], equipmentIcons[0]);
        currentEquipmentIcon.sprite = equipmentIcons[0];

        foreach (var image in possibleEquipmentsImages) {
            if (image.sprite.name == equipmentIcons[0].name) {
                image.sprite = equipmentIcons[pos];
            }
        }
        
        HidePossibleEquipments();
    }

    /// <summary>
    /// Metoda, odpowiadająca za chowanie możliwych broń
    /// </summary>
    private void SwitchPossibleWeapons() {
        if (isPossibleWeaponsShowed) {
            HidePossibleWeapons();
        } else {
            HidePossibleEquipments();
            ShowPossibleWeapons();
        }
    }

    /// <summary>
    /// Metoda, odpowiadająca za chowanie możliwego wyposażenia
    /// </summary>
    private void SwitchPossibleEquipments() {
        if (isPossibleEquipmentsShowed) {
            HidePossibleEquipments();
        } else {
            HidePossibleWeapons();
            ShowPossibleEquipments();
        }
    }

    /// <summary>
    /// Wyświetla możliwe broni
    /// </summary>
    private void ShowPossibleWeapons() {
        possibleWeapons.SetActive(true);
        isPossibleWeaponsShowed = true;
        weaponExtendedArrowIcon.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    /// <summary>
    /// Chowa możliwe broni
    /// </summary>
    private void HidePossibleWeapons() {
        possibleWeapons.SetActive(false);
        isPossibleWeaponsShowed = false;
        weaponExtendedArrowIcon.transform.rotation = new Quaternion(0, 0, 180, 0);
    }

    /// <summary>
    /// Wyświetla możliwe wyposażenia
    /// </summary>
    private void ShowPossibleEquipments() {
        possibleEquipments.SetActive(true);
        isPossibleEquipmentsShowed = true;
        equipmentExtendedArrowIcon.transform.rotation = new Quaternion(0, 0, 0, 0);

        foreach (var image in possibleEquipmentsImages) {
            if (image.sprite.name.ToLower().Contains("jetpack")) {
                if (_characterController.IsJetpackPicked) {
                    var temp = image.color;
                    temp.a = 1f;
                    image.color = temp;
                }
            }
        }
    }

    /// <summary>
    /// Chowa możliwe wyposażenia
    /// </summary>
    private void HidePossibleEquipments() {
        possibleEquipments.SetActive(false);
        isPossibleEquipmentsShowed = false;
        equipmentExtendedArrowIcon.transform.rotation = new Quaternion(0, 0, 180, 0);
    }
}
