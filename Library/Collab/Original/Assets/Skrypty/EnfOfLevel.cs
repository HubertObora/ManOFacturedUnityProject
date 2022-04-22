using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnfOfLevel : MonoBehaviour
{
    [SerializeField] private GameObject PodsumowaniePoziomu;
    [SerializeField] private GameObject InfoBezSprezyny;
    [SerializeField] public GameObject InfoSprezyna;
    [SerializeField] public GameObject InfoDead;
    [SerializeField] private GameObject Ulepszenia;
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
    
    public void ShowUpgrades()
    {
        Ulepszenia.SetActive(true);
        PodsumowaniePoziomu.SetActive(false);
    }

    public void HideUpgrades() {
        PodsumowaniePoziomu.SetActive(true);
        Ulepszenia.SetActive(false);
    }
    
    public void Powrot()
    {
        Ulepszenia.SetActive(false);
        PodsumowaniePoziomu.SetActive(false);
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
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
    private void PauseGame()
    {
        PodsumowaniePoziomu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void NewGameButton()
    {
        SceneManager.LoadScene("Level 1");
        Time.timeScale = 1f;
    }
}
