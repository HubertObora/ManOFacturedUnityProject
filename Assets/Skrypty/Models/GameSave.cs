using System;
using System.Collections.Generic;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje plik z zapisanym stanem gry
  /// </summary>
  [Serializable]
  public class GameSave {
    public int SceneId { get; set; }
    public PlayerSaveData PlayerSaveData { get; set; }
    public List<EnemySaveData> EnemiesSaveData { get; set; }
    public BatteriesSaveData BatteriesSaveData { get; set; }
    public GearsSaveData GearsSaveData { get; set; }
    public UpgradesSaveData UpgradesSaveData { get; set; }
    public bool IsEndOfLevelShoved { get; set; }
    public int CollectedGears { get; set; }
    public TimeSpan LevelPlayTime { get; set; }
  }
}