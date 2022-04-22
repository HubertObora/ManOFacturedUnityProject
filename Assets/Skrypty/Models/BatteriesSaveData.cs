using System;
using UnityEngine;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje baterie na mapie dla zapisywania ich pozycji w plik
  /// </summary>
  [Serializable]
  public class BatteriesSaveData : PickableObjectsSaveData {
    public BatteriesSaveData(GameObject[] gearsPositions) : base(gearsPositions) {}
  }
}