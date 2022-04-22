using System.Collections.Generic;
using Skrypty.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Klasa, obsługująca kupowanie ulepszeń
/// </summary>
public class UpgradesShop : MonoBehaviour {
    [SerializeField] private Button hpUpgradeBtn;
    [SerializeField] private Button fallUpgradeBtn;
    [SerializeField] private Button jetPackUpgradeBtn;
    [SerializeField] private Text collectedGears;

    [SerializeField] private Camera camera;

    private Image _hpUpgradeIcon;
    public static int HpUpgradePrice = 5;
    public static int HpUpgradeLvl = 0;
    
    private Image _fallUpgradeIcon;
    public static int FallUpgradePrice = 5;
    public static int FallUpgradeLvl = 0;
    
    private Image _jetPackUpgradeIcon;
    public static int JetPackUpgradePrice = 5;
    public static int JetPackUpgradeLvl = 0;
    
    public delegate void UpgradesHandler(int lvl);

    public static event UpgradesHandler OnHpUpgradeBuy;
    public static event UpgradesHandler OnFallUpgradeBuy;
    public static event UpgradesHandler OnJetPackUpgradeBuy;

    [SerializeField] private GameObject upgradeOnHoverText;
    [SerializeField] private Vector3 onHoverTextOffset = new Vector3(200, -80, 0);

    private void OnEnable() {
        _hpUpgradeIcon = hpUpgradeBtn.GetComponentsInChildren<Image>()[1];
        _fallUpgradeIcon = fallUpgradeBtn.GetComponentsInChildren<Image>()[1];
        _jetPackUpgradeIcon = jetPackUpgradeBtn.GetComponentsInChildren<Image>()[1];

        collectedGears.text = GameController.CollectedGears.ToString();

        hpUpgradeBtn.onClick.AddListener(BuyHpUpgrade);
        fallUpgradeBtn.onClick.AddListener(BuyFallUpgrade);
        jetPackUpgradeBtn.onClick.AddListener(BuyJetPackUpgrade);
        
        SetIconsTransparency();

        GameController.OnTakeGears += _ => {
            collectedGears.text = GameController.CollectedGears.ToString();
        };
    }

    private void Update() {
        ShowOnHoverText(GetEventSystemRaycastResults());
    }

    /// <summary>
    /// Wyświetla tekst przy najechaniu przycisku myszy z informacjami dotyczącymi ulepszenia
    /// </summary>
    /// <param name="eventSystemRaycastResults">Lista z elementami, które znajdują się pod przyciskiem myszy</param>
    private void ShowOnHoverText(List<RaycastResult> eventSystemRaycastResults)
    {
        if (eventSystemRaycastResults.Count > 0) {
            RaycastResult curRaycastResult = eventSystemRaycastResults[0];

            if (curRaycastResult.gameObject.layer == LayerMask.NameToLayer("UI")) {
                if (curRaycastResult.gameObject.name == "Icon") {
                    var image = curRaycastResult.gameObject.GetComponent<Image>();

                    if (image != null) {
                        var pos = camera.ScreenToWorldPoint(Input.mousePosition + onHoverTextOffset);
                        pos.z = 1;
                        switch (image.sprite.name) {
                            case "HpIcon 1":
                                upgradeOnHoverText.SetActive(true);
                                upgradeOnHoverText.gameObject.transform.position = pos; 
                                upgradeOnHoverText.GetComponentsInChildren<Text>()[0].text = $"Aktualny poziom: {HpUpgradeLvl}\nCena ulepszenia: {HpUpgradePrice}\nZwiększ maksymalną liczbę punktów zdrowia";
                                break;
                            case "FallingIcon":
                                upgradeOnHoverText.SetActive(true);
                                upgradeOnHoverText.gameObject.transform.position = pos;
                                upgradeOnHoverText.GetComponentsInChildren<Text>()[0].text = $"Aktualny poziom: {FallUpgradeLvl}\nCena ulepszenia: {FallUpgradePrice}\nZmniejsz obrażenia od upadku z większej wysokości";
                                break;
                            case "CanisterIcon":
                                upgradeOnHoverText.SetActive(true);
                                upgradeOnHoverText.gameObject.transform.position = pos;
                                upgradeOnHoverText.GetComponentsInChildren<Text>()[0].text = $"Aktualny poziom: {JetPackUpgradeLvl}\nCena ulepszenia: {JetPackUpgradePrice}\nZwiększ dostępną ilość paliwa do jetpacka (dostępny na 4 planszy)";
                                break;
                        }
                    }
                } else {
                    upgradeOnHoverText.SetActive(false);
                }
            }   
        }
    }
    
    /// <summary>
    /// Otrzymuje elementy, które znajdują się pod przyciskiem myszy
    /// </summary>
    /// <returns>Elementy, które znajdują się pod przyciskiem myszy</returns>
    private List<RaycastResult> GetEventSystemRaycastResults()
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) {position = Input.mousePosition}, raycastResults);
        return raycastResults;
    }

    /// <summary>
    /// Ustala przezroczystość dla ikon ulepszeń w zależności od tego, czy starczy graczu zębatek dla kupowania tego ulepszenia
    /// </summary>
    private void SetIconsTransparency() {
        if (GameController.CollectedGears < HpUpgradePrice) {
            hpUpgradeBtn.enabled = false;
            var tempColor = _hpUpgradeIcon.color;
            tempColor.a = 0.7f;
            _hpUpgradeIcon.color = tempColor;
        }

        if (GameController.CollectedGears < FallUpgradePrice) {
            fallUpgradeBtn.enabled = false;
            var tempColor = _fallUpgradeIcon.color;
            tempColor.a = 0.7f;
            _fallUpgradeIcon.color = tempColor;
        }

        if (GameController.CollectedGears < JetPackUpgradePrice) {
            jetPackUpgradeBtn.enabled = false;
            var tempColor = _jetPackUpgradeIcon.color;
            tempColor.a = 0.7f;
            _jetPackUpgradeIcon.color = tempColor;
        }
    }

    /// <summary>
    /// Metoda, pozwalająca na kupowanie ulepszenia zdrowia
    /// </summary>
    private void BuyHpUpgrade() {
        if (GameController.TakeGears(HpUpgradePrice)) {
            HpUpgradeLvl++;
            HpUpgradePrice += 10;
            if (OnHpUpgradeBuy != null) OnHpUpgradeBuy.Invoke(HpUpgradeLvl);
        }
        
        SetIconsTransparency();
    }

    /// <summary>
    /// Metoda, pozwalająca na kupowanie ulepszenia redukcji obrażeń od upadku
    /// </summary>
    private void BuyFallUpgrade() {
        if (GameController.TakeGears(FallUpgradePrice)) {
            FallUpgradeLvl++;
            FallUpgradePrice += 10;
            if (OnFallUpgradeBuy != null) OnFallUpgradeBuy.Invoke(FallUpgradeLvl);
        }
        
        SetIconsTransparency();
    }

    /// <summary>
    /// Metoda, pozwalająca na kupowanie ulepszenia JetPacka
    /// </summary>
    private void BuyJetPackUpgrade() {
        if (GameController.TakeGears(JetPackUpgradePrice)) {
            JetPackUpgradeLvl++;
            JetPackUpgradePrice += 10;
            if (OnJetPackUpgradeBuy != null) OnJetPackUpgradeBuy.Invoke(JetPackUpgradeLvl);
        }
        
        SetIconsTransparency();
    }

    /// <summary>
    /// Zastosowuje zmiany zapisane w pliku ze stanem gry przy kontynuacji gry
    /// </summary>
    /// <param name="saveData">Obiekt z informacjami o zapisanym stanie ulepszeń</param>
    public static void ApplySaveData(UpgradesSaveData saveData) {
        HpUpgradeLvl = saveData.HpUpgradeLvl;
        HpUpgradePrice = saveData.HpUpgradePrice;
        FallUpgradeLvl = saveData.FallUpgradeLvl;
        FallUpgradePrice = saveData.FallUpgradePrice;
        JetPackUpgradeLvl = saveData.JetPackUpgradeLvl;
        JetPackUpgradePrice = saveData.JetPackUpgradePrice;
    }
}
