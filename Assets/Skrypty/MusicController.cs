using UnityEngine;
using Random = System.Random;

/// <summary>
/// Klasa, obsługujące muzyki
/// </summary>
public class MusicController : MonoBehaviour {
    [SerializeField] private AudioClip[] backgroundSongs;
    private AudioSource _audioSource;
    private AudioClip _previousSong = null;
    private static Random _random = new Random();
    
    void Start() {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null) {
            _audioSource.volume = SettingsManager.gameMusicVolume;
        }
    }

    private void FixedUpdate() {
        if (!_audioSource.isPlaying) {
            PlayRandomSong();
        }
    }

    /// <summary>
    /// Metoda odpowiadająca za odtwarzania losowej piosenki
    /// </summary>
    private void PlayRandomSong() {
        AudioClip song = null;

        if (backgroundSongs.Length > 1) {
            do {
                var randomIndex = _random.Next(0, backgroundSongs.Length);
                song = backgroundSongs[randomIndex];
            } while (_previousSong != song);   
        } else {
            var randomIndex = _random.Next(0, backgroundSongs.Length);
            song = backgroundSongs[randomIndex];
        }

        _audioSource.PlayOneShot(song);
    }
}
