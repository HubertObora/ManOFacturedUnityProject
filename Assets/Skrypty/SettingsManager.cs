using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IniParser;
using IniParser.Configuration;
using Skrypty.Enums;
using UnityEngine;

/// <summary>
/// Klasa, odpowiadająca za zachowywanie, czytanie i zapisywanie ustawień do pliku
/// </summary>
public static class SettingsManager {
    private static string PathToSettingFile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "settings.ini";
    
    public static Resolution gameResolution = Screen.currentResolution;
    public static GraphicsSettingsEnum gameGraphics = GraphicsSettingsEnum.Medium;
    public static float gameMusicVolume = 0.5f;
    public static float gameEffectsVolume = 0.5f;
    public static bool gameIsFullScreen = Screen.fullScreen;
    
    public static Resolution defaultResolution = Screen.currentResolution;
    public static GraphicsSettingsEnum defaultGraphics = GraphicsSettingsEnum.Medium;
    public static float defaultMusicVolume = 0.5f;
    public static float defaultEffectsVolume = 0.5f;
    public static bool defaultIsFullScreen = true;

    public static event Action OnSettingsSaved;

    /// <summary>
    /// Zapisuje ustawienia do pliku
    /// </summary>
    public static void SaveSettings() {
        if (!File.Exists(PathToSettingFile)) {
            File.Create(PathToSettingFile);
        }
        
        var data = new IniData {
            CreateSectionsIfTheyDontExist = true
        };
        var values = GetFieldValues(typeof(SettingsManager));

        foreach (var value in values) {
            data["Settings"][value.Key] = value.Value;
        }
        var settings = new IniDataFormatter().Format(data, new IniFormattingConfiguration());
            
        using (StreamWriter outputFile = new StreamWriter(PathToSettingFile, false))
        {
            outputFile.WriteLine(settings);
            if (OnSettingsSaved != null) OnSettingsSaved.Invoke();
        }
    }

    /// <summary>
    /// Czyta ustawienia z pliku
    /// </summary>
    public static void LoadSettings() {
        if (File.Exists(PathToSettingFile)) {
            var parser = new IniDataParser();
            var data = parser.Parse(File.ReadAllText(PathToSettingFile));

            foreach (var property in data["Settings"]) {
                switch (property.Key) {
                    case nameof(gameResolution):
                        var parsedString = ParseResolutionString(property.Value);
                        gameResolution = new Resolution() {
                            width = parsedString[0],
                            height = parsedString[1],
                            refreshRate = parsedString[2]
                        };
                        Screen.SetResolution(gameResolution.width, gameResolution.height, gameIsFullScreen);
                        break;
                    case nameof(gameGraphics):
                        gameGraphics = (GraphicsSettingsEnum)Enum.Parse(typeof(GraphicsSettingsEnum), property.Value);
                        break;
                    case nameof(gameMusicVolume):
                        gameMusicVolume = float.Parse(property.Value);
                        break;
                    case nameof(gameEffectsVolume):
                        gameEffectsVolume = float.Parse(property.Value);
                        break;
                    case nameof(gameIsFullScreen):
                        gameIsFullScreen = bool.Parse(property.Value);
                        break;
                }
            }
        } else {
            SaveSettings();
        }
    }

    /// <summary>
    /// Parsuje rozszerzenie zapisane w pliku
    /// </summary>
    /// <param name="resolutionString">Wrtośc rozszerzenia, zapisana w pliku</param>
    /// <returns>Lista z elementami rozszerzenia(wysokość, szerokośż i ilość HZ)</returns>
    private static List<int> ParseResolutionString(string resolutionString) {
        var result = new List<int>(3);

        var temp = "";

        for (int i = 0; i < resolutionString.Length; i++) {
            var chr = resolutionString[i];
            
            if (chr != 'x' && chr != '@' && chr != ' ' && chr != 'H' && chr != 'z') {
                temp += chr;
            } else {
                if (temp != "") {
                    result.Add(int.Parse(temp));
                }

                temp = "";
            }
        }

        return result;
    }

    /// <summary>
    /// Parsuje pola klasy w postać słownika
    /// </summary>
    /// <param name="type">Typ, którego pola muszą być przekształcone</param>
    /// <returns>Słownik z polami podanego typu</returns>
    private static Dictionary<string, string> GetFieldValues(Type type) {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToDictionary(f => f.Name, f => f.GetValue(null).ToString());
    }
}
