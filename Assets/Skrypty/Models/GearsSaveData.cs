using System;
using UnityEngine;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje zębatki na mapie dla zapisywania ich pozycji w plik
  /// </summary>
  [Serializable]
  public class GearsSaveData : PickableObjectsSaveData {
    public GearsSaveData(GameObject[] gearsPositions) : base(gearsPositions) {}
  }
}