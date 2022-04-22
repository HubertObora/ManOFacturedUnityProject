using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Klasa, odpowiadająca za informacji, wyświetlone w końcu poziomu
/// </summary>
public class EnfOfLevel : MonoBehaviour
{
    [SerializeField] private GameObject PodsumowaniePoziomu;
    [SerializeField] private GameObject InfoBezSprezyny;
    [SerializeField] public GameObject InfoSprezyna;
    [SerializeField] public GameObject InfoDead;
    [SerializeField] private GameObject Ulepszenia;
    [SerializeField] private Text collectedGears;
    [SerializeField] private Text timeOfLevel;
    public CharacterController2D controller;
    private string sceneName;
    private AudioSource _audioSource;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null) {
            _audioSource.volume = SettingsManager.gameEffectsVolume;
        }
    }

    void Update()
    {
        if(InfoBezSprezyny.active == true || InfoSprezyna.active == true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                OK();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            _audioSource.Play();
            collectedGears.text = GameController.CollectedGears.ToString();
            timeOfLevel.text = GameController.LevelPlayTime.ToString(@"mm\:ss");
            if (sceneName == "Level 1")
            {
                if (controller.canJump == true)
                {
                    PauseGame();
                }
                else
                {
                    InfoWithoutJump();
                }
            }
            else if(sceneName=="Level 2")
            {
                PauseGame();
            }
            else if (sceneName == "Level 3")
            {
                PauseGame();
            }
            else if (sceneName == "Level 4")
            {
                PauseGame();
            }
            else if (sceneName == "Level 5")
            {
                PauseGame();
            }
        }
    }
    
    /// <summary>
    /// Wyświetla okno z informacją o tym, że gracz nie zebrał sprężynę
    /// </summary>
    public void InfoWithoutJump()
    {
        InfoBezSprezyny.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void OK()
    {
        InfoBezSprezyny.SetActive(false);
        InfoSprezyna.SetActive(false);
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Wyświetla okno z ulepszeniami
    /// </summary>
    public void ShowUpgrades()
    {
        Ulepszenia.SetActive(true);
        PodsumowaniePoziomu.SetActive(false);
    }

    /// <summary>
    /// Chowa okno z ulepszeniami
    /// </summary>
    public void HideUpgrades() {
        PodsumowaniePoziomu.SetActive(true);
        Ulepszenia.SetActive(false);
    }
    
    /// <summary>
    /// Wraca gracza do głównego menu
    /// </summary>
    public void Powrot()
    {
        Ulepszenia.SetActive(false);
        PodsumowaniePoziomu.SetActive(false);
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Przenosi gracza na kolejny poziom
    /// </summary>
    public void KolejnyPoziom()
    {
        Ulepszenia.SetActive(false);
        PodsumowaniePoziomu.SetActive(false);
        if (sceneName == "Level 1")
        {
            SceneManager.LoadScene("Level 2");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 2")
        {
            SceneManager.LoadScene("Level 3");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 3")
        {
            SceneManager.LoadScene("Level 4");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 4")
        {
            SceneManager.LoadScene("Level 5");
            Time.timeScale = 1f;
        }
    }
    
    /// <summary>
    /// Powtarza poziom od nowa
    /// </summary>
    public void PowtorzPoziom()
    {
        InfoDead.SetActive(false);
        if (sceneName == "Level 1")
        {
            SceneManager.LoadScene("Level 1");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 2")
        {
            SceneManager.LoadScene("Level 2");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 3")
        {
            SceneManager.LoadScene("Level 3");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 4")
        {
            SceneManager.LoadScene("Level 4");
            Time.timeScale = 1f;
        }
        else if (sceneName == "Level 5")
        {
            SceneManager.LoadScene("Level 5");
            Time.timeScale = 1f;
        }
    }
    
    /// <summary>
    /// Wyświetla okno z podsumowaniem poziomu
    /// </summary>
    private void PauseGame()
    {
        PodsumowaniePoziomu.SetActive(true);
        Time.timeScale = 0f;
        GameSaver.SaveGame();
    }
}
