using System;
using UnityEngine;

/// <summary>
/// Klasa, obsługująca plaftormy laserowe
/// </summary>
public class LaserPlatformController : MonoBehaviour {
    [SerializeField] private GameObject laserTop;
    [SerializeField] private GameObject laserMiddle;
    [SerializeField] private GameObject laserBottom;
    [SerializeField] private Transform weapon;
    [SerializeField] private int minLaserTime = 2;
    [SerializeField] private int maxLaserTime = 5;
    [SerializeField] private int minLaserPeaceTime = 2;
    [SerializeField] private int maxLaserPeaceTime = 5;

    private GameObject[] _laserObjects;
    private System.Random _random = new System.Random();
    private float _nextTimeToSpawnLasers = 0f;

    private void Start() {
        var bottomPlatformInfo = Physics2D.Raycast( weapon.position, Vector2.down, 10);

        if (bottomPlatformInfo.collider != null) {
            if (bottomPlatformInfo.collider.CompareTag("LaserPlatform")) {
                _laserObjects = new GameObject[(int)Math.Ceiling(bottomPlatformInfo.distance)];
            }
        }
    }

    private void FixedUpdate() {
        Debug.DrawRay(weapon.position, Vector3.down * 10, Color.green);
        
        if (_laserObjects != null) {
            if (_laserObjects[0] == null && Time.time > _nextTimeToSpawnLasers) {
                InstantiateLaser(_laserObjects.Length);
            }   
        }
    }

    /// <summary>
    /// Tworzy lasery
    /// </summary>
    /// <param name="distance">Dystancja, do której lasery muszą być tworzone</param>
    private void InstantiateLaser(int distance) {
        var timeToDestroy = _random.Next(minLaserTime, maxLaserTime);
        for (int i = 0; i < distance; i++) {
            GameObject laserToInstantiate;

            if (i == 0) {
                laserToInstantiate = laserTop;
            } else if (i == distance - 1) {
                laserToInstantiate = laserBottom;
            } else {
                laserToInstantiate = laserMiddle;
            }
            
            var laserObj =
                    Instantiate(
                            laserToInstantiate,
                            new Vector3(transform.position.x, transform.position.y - (i + 1), transform.position.z),
                            new Quaternion(), transform);
            _laserObjects[i] = laserObj;
            Invoke(nameof(DestroyLasers), timeToDestroy);
        }
    }

    /// <summary>
    /// Usuwa lasery
    /// </summary>
    private void DestroyLasers() {
        foreach (var laserObject in _laserObjects) {
            Destroy(laserObject);
        }
        _nextTimeToSpawnLasers = Time.time + _random.Next(minLaserPeaceTime, maxLaserPeaceTime);
    }
}
