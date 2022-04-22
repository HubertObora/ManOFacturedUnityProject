using System;
using UnityEngine;

/// <summary>
/// Klasa odpowiadająca za obsługę platform elektrycznych
/// </summary>
public class ElectricPlatformController : MonoBehaviour {
    [SerializeField] private GameObject electricRight;
    [SerializeField] private GameObject electricMiddle;
    [SerializeField] private GameObject electricLeft;
    [SerializeField] private Transform weapon;
    [SerializeField] private int minLaserTime = 2;
    [SerializeField] private int maxLaserTime = 5;
    [SerializeField] private int minElectricityPeaceTime = 2;
    [SerializeField] private int maxElectricityPeaceTime = 5;
    
    private GameObject[] _electricityObjects;
    private System.Random _random = new System.Random();
    private float _nextTimeToSpawnElectricity = 0f;
    
    void Start() {
        var bottomPlatformInfo = Physics2D.Raycast( weapon.position, Vector2.left, 10);

        if (bottomPlatformInfo.collider != null) {
            if (bottomPlatformInfo.collider.CompareTag("ElectricPlatform")) {
                _electricityObjects = new GameObject[(int)Math.Ceiling(bottomPlatformInfo.distance)];
            }
        }
    }
    
    void FixedUpdate() {
        Debug.DrawRay(weapon.position, Vector3.left * 10, Color.green);
        
        if (_electricityObjects != null) {
            if (_electricityObjects[0] == null && Time.time > _nextTimeToSpawnElectricity) {
                InstantiateElectricity(_electricityObjects.Length);
            }   
        }
    }
    
    /// <summary>
    /// Tworzy elektryczność
    /// </summary>
    /// <param name="distance">Dystancja, do której elektryczność muszi być tworzona</param>
    private void InstantiateElectricity(int distance) {
        var timeToDestroy = _random.Next(minLaserTime, maxLaserTime);
        for (int i = 0; i < distance; i++) {
            GameObject electricityToInstantiate;

            if (i == 0) {
                electricityToInstantiate = electricRight;
            } else if (i == distance - 1) {
                electricityToInstantiate = electricLeft;
            } else {
                electricityToInstantiate = electricMiddle;
            }
            
            var electricityObj =
                    Instantiate(
                            electricityToInstantiate,
                            new Vector3(transform.position.x - (i), transform.position.y - 0.25f, transform.position.z),
                            new Quaternion(), transform);
            _electricityObjects[i] = electricityObj;
            Invoke(nameof(DestroyElectricity), timeToDestroy);   
        }
    }
    
    /// <summary>
    /// Usuwa elektryczność
    /// </summary>
    private void DestroyElectricity() {
        foreach (var electricityObject in _electricityObjects) {
            Destroy(electricityObject);
        }
        _nextTimeToSpawnElectricity = Time.time + _random.Next(minElectricityPeaceTime, maxElectricityPeaceTime);
    }
}
