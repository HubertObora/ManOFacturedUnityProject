using System;
using Skrypty.Enums;
using Skrypty.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa, obsługująca aktualny stan gry
/// </summary>
public class GameController : MonoBehaviour {
    [SerializeField] private Text playTime;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private GameObject gearPrefab;
    private CharacterController2D playerController;
    private static GameObject _endOfLevel;
    public static DateTime LevelStartTime { get; private set; }
    public static TimeSpan LevelPlayTime { get; private set; }
    public static int CollectedGears { get; set; }
    public static bool IsGamePaused { get; set; }
    public static string GameVersion { get; } = "1.0";
    public static event Action<int> OnTakeGears;

    /// <summary>
    /// Odpowiada za zmniejszenie ilości zebranych zębatek
    /// </summary>
    /// <param name="amount">Ilość zębatek, która musi być odjęta</param>
    /// <returns>Wartość logiczną, reprezentującą, czy operacja udała się</returns>
    public static bool TakeGears(int amount) {
        if (CollectedGears - amount >= 0) {
            CollectedGears -= amount;
            if (OnTakeGears != null) OnTakeGears.Invoke(amount);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Zastosowuje zmiany zapisane w pliku ze stanem gry przy kontynuacji gry
    /// </summary>
    /// <param name="gameSave">Obiekt z zapisanym stanem gry</param>
    public static void ApplySaveData(GameSave gameSave) {
        if (gameSave != null) {
            if (_endOfLevel != null) {
                _endOfLevel.SetActive(gameSave.IsEndOfLevelShoved);
            }

            CollectedGears = gameSave.CollectedGears;
            if (OnTakeGears != null) OnTakeGears.Invoke(-CollectedGears);
            LevelPlayTime = gameSave.LevelPlayTime;
        }
    }

    private void Start() {
        if (PlayerPrefs.HasKey("IsGameContinued")) {
            if (PlayerPrefs.GetInt("IsGameContinued") == 1) {
                if (GameSaver.GameSave.SceneId == SceneManager.GetActiveScene().buildIndex) {
                    GameSaver.LoadGame(enemyPrefab, batteryPrefab, gearPrefab);
                    PlayerPrefs.SetInt("IsGameContinued", 0);
                }
            }
        }
        
        playerController = player.GetComponent<CharacterController2D>();

        if (playerController != null) {
            playerController.OnItemCollected += AddItem;
        }
        
        LevelStartTime = DateTime.Now;
        InvokeRepeating(nameof(SetPlayTime), 0, 1f);
        
        _endOfLevel = GameObject.FindGameObjectWithTag("EndOfLevel");
    }
    
    /// <summary>
    /// Ustala czas gry na poziomie
    /// </summary>
    private void SetPlayTime() {
        LevelPlayTime = DateTime.Now - LevelStartTime;
        playTime.text = LevelPlayTime.ToString(@"mm\:ss");
    }

    /// <summary>
    /// Odpowiada za dodawanie pewnych obiektów, które gracz może zbierać
    /// </summary>
    /// <param name="type">Typ zebranego obiektu</param>
    /// <param name="value">Ilośc zebranych obiektów pewnego typu</param>
    private void AddItem(PickUpItemsEnum type, int value) {
        switch (type) {
            case PickUpItemsEnum.Gears:
                CollectedGears += value;
                Debug.Log($"Collected gear. Total gears: {CollectedGears}");
                break;
        }
    }
}
