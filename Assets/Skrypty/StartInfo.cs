using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasa, obsługująca wyświetlanie instrukcji przy rozpoczęciu gry
/// </summary>
public class StartInfo : MonoBehaviour
{
    [SerializeField] private GameObject startInfo;
    private string sceneName;
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        if (sceneName == "Level 1")
        {
            startInfo.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// Metoda, obsługująca naciskanie przycisku potwierdzenia
    /// </summary>
    public void OK()
    {
        startInfo.SetActive(false);
        Time.timeScale = 1f;
    }
}
