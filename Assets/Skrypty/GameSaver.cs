using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Skrypty.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa, odpowiadająca za zapisywanie i odczytywanie stanu gry
/// </summary>
public static class GameSaver {
    private static string PathToSavesFolder = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Saves";
    private static string SavePath = PathToSavesFolder + Path.DirectorySeparatorChar + "save.frog";
    public static bool IsGameSaveExists { get; private set; } = false;
    public static GameSave GameSave { get; set; }


    static GameSaver() {
        if (Directory.Exists(PathToSavesFolder)) {
            if (File.Exists(SavePath)) {
                IsGameSaveExists = true;
                GameSave = ReadSaveFile();
            }
        }
    }

    /// <summary>
    /// Zapisuje stan gry do pliku
    /// </summary>
    public static void SaveGame() {
        var gameSave = GenerateGameSave();

        if (!Directory.Exists(PathToSavesFolder)) {
            Directory.CreateDirectory(PathToSavesFolder);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (var file = File.Open(SavePath, FileMode.OpenOrCreate))
        {
            formatter.Serialize(file, gameSave);
        }
    }

    /// <summary>
    /// Odczytywuje stan gry z pliku
    /// </summary>
    /// <param name="enemyPrefab">Prefab przeciwnika</param>
    /// <param name="batteryPrefab">Prefab baterii</param>
    /// <param name="gearPrefab">Prefab zębatki</param>
    public static void LoadGame(GameObject enemyPrefab, GameObject batteryPrefab, GameObject gearPrefab) {
        if (Directory.Exists(PathToSavesFolder)) {
            if (File.Exists(SavePath)) {
                var gameSave = ReadSaveFile();

                if (gameSave != null) {
                    DeleteSavableObjects();
                    InstantiateSavedObjects(gameSave, enemyPrefab, batteryPrefab, gearPrefab);
                    UpgradesShop.ApplySaveData(gameSave.UpgradesSaveData);
                    GameController.ApplySaveData(gameSave);
                }
            }
        }
    }

    /// <summary>
    /// Czyta plik z zapisanym stanem gry
    /// </summary>
    /// <returns></returns>
    private static GameSave ReadSaveFile() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SavePath, FileMode.Open);
        var gameSave = formatter.Deserialize(stream) as GameSave;
        stream.Close();

        return gameSave;
    }
    
    /// <summary>
    /// Umieszcza zapisane obiekty na scenie
    /// </summary>
    /// <param name="gameSave">Obiekt z zapisanym stanem gry</param>
    /// <param name="enemyPrefab">Prefab przeciwnika</param>
    /// <param name="batteryPrefab">Prefab baterii</param>
    /// <param name="gearPrefab">Prefab zębatki</param>
    private static void InstantiateSavedObjects(GameSave gameSave, GameObject enemyPrefab, GameObject batteryPrefab, GameObject gearPrefab) {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerController = player.GetComponent<CharacterController2D>();
        player.transform.position = new Vector3(gameSave.PlayerSaveData.Position[0],
                                                gameSave.PlayerSaveData.Position[1],
                                                gameSave.PlayerSaveData.Position[2]);
        player.transform.localScale = new Vector3(gameSave.PlayerSaveData.Scale[0], gameSave.PlayerSaveData.Scale[1],
                                                  gameSave.PlayerSaveData.Scale[2]);
        playerController.ApplySavedData(gameSave.PlayerSaveData);

        foreach (var enemySaveData in gameSave.EnemiesSaveData) {
            var enemy = Object.Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(enemySaveData.Position[0],
                                                   enemySaveData.Position[1],
                                                   enemySaveData.Position[2]);
            enemy.transform.rotation = new Quaternion(enemySaveData.Rotation[0],
                                                      enemySaveData.Rotation[1],
                                                      enemySaveData.Rotation[2],
                                                      enemySaveData.Rotation[3]);
            var enemyController = enemy.GetComponent<EnemyController2D>();
            enemyController.ApplySavedData(enemySaveData);
        }

        foreach (var gearPosition in gameSave.GearsSaveData.Positions) {
            Object.Instantiate(gearPrefab).transform.position = new Vector3(
                    gearPosition[0],
                    gearPosition[1],
                    gearPosition[2]
            );
        }

        foreach (var batteryPosition in gameSave.BatteriesSaveData.Positions) {
            Object.Instantiate(batteryPrefab).transform.position = new Vector3(
                    batteryPosition[0],
                    batteryPosition[1],
                    batteryPosition[2]
            );
        }
    }
    
    /// <summary>
    /// Usuwa obiekty, które mogą być zapisywanie do pliku ze sceny
    /// </summary>
    private static void DeleteSavableObjects() {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var batteries = GameObject.FindGameObjectsWithTag("Bateria");
        var gears = GameObject.FindGameObjectsWithTag("Zebatka");

        foreach (var enemy in enemies) {
            Object.Destroy(enemy);
        }

        foreach (var battery in batteries) {
            Object.Destroy(battery);
        }

        foreach (var gear in gears) {
            Object.Destroy(gear);
        }
    }

    /// <summary>
    /// Tworzy obiekt z zapisanym stanem gry
    /// </summary>
    /// <returns>Utworzony obiekt z zapisanym stanem gry</returns>
    private static GameSave GenerateGameSave() {
        var player = GameObject.FindGameObjectWithTag("Player");
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var batteries = GameObject.FindGameObjectsWithTag("Bateria");
        var gears = GameObject.FindGameObjectsWithTag("Zebatka");
        var endOfLevel = GameObject.FindGameObjectWithTag("EndOfLevel");
        var isEndOfLevelShoved = false;
        var playerSaveData = new PlayerSaveData(player.transform, player.GetComponent<CharacterController2D>());
        var gearsSaveData = new GearsSaveData(gears);
        var batteriesSaveData = new BatteriesSaveData(batteries);
        var enemiesSaveData = new List<EnemySaveData>(enemies.Length);

        foreach (var enemy in enemies) {
            enemiesSaveData.Add(new EnemySaveData(enemy.transform, enemy.GetComponent<EnemyController2D>()));
        }

        if (endOfLevel != null) {
            isEndOfLevelShoved = endOfLevel.activeSelf;
        }
        
        var gameSave = new GameSave() {
            SceneId = SceneManager.GetActiveScene().buildIndex,
            PlayerSaveData = playerSaveData,
            GearsSaveData = gearsSaveData,
            BatteriesSaveData = batteriesSaveData,
            EnemiesSaveData = enemiesSaveData,
            UpgradesSaveData = new UpgradesSaveData(),
            IsEndOfLevelShoved = isEndOfLevelShoved,
            CollectedGears = GameController.CollectedGears,
            LevelPlayTime = GameController.LevelPlayTime
        };

        return gameSave;
    }
}
