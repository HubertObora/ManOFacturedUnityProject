using System;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje dane zdobytych ulepszeń gracza dla zapisywania ich stanu w plik
  /// </summary>
  [Serializable]
  public class UpgradesSaveData {
    public int HpUpgradeLvl { get; set; }
    public int HpUpgradePrice { get; set; }
    public int FallUpgradeLvl { get; set; }
    public int FallUpgradePrice { get; set; }
    public int JetPackUpgradeLvl { get; set; }
    public int JetPackUpgradePrice { get; set; }

    public UpgradesSaveData() {
      HpUpgradeLvl = UpgradesShop.HpUpgradeLvl;
      HpUpgradePrice = UpgradesShop.HpUpgradePrice;
      FallUpgradeLvl = UpgradesShop.FallUpgradeLvl;
      FallUpgradePrice = UpgradesShop.FallUpgradePrice;
      JetPackUpgradeLvl = UpgradesShop.JetPackUpgradeLvl;
      JetPackUpgradePrice = UpgradesShop.JetPackUpgradePrice;
    }
  }
}