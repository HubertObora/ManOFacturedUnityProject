using System;
using UnityEngine;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje podstawową klasę dla zapisywania danych stanu podejmowanych obiektów
  /// </summary>
  [Serializable]
  public abstract class PickableObjectsSaveData {
    public float[][] Positions { get; set; }

    public PickableObjectsSaveData(GameObject[] gearsPositions) {
      Positions = new float[gearsPositions.Length][];

      for (int i = 0; i < Positions.Length; i++) {
        Positions[i] = new[] {
          gearsPositions[i].transform.position.x,
          gearsPositions[i].transform.position.y,
          gearsPositions[i].transform.position.z
        };
      }
    }
  }
}