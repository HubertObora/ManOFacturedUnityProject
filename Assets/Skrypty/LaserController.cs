using UnityEngine;

/// <summary>
/// Klasa, obsługująca lasery
/// </summary>
public class LaserController : MonoBehaviour {
    [SerializeField] private float damage = 15;

    private AudioSource _audioSource;

    private void Start() {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null) {
            _audioSource.volume = SettingsManager.gameEffectsVolume;
            
            SettingsManager.OnSettingsSaved += () => {
                _audioSource.volume = SettingsManager.gameEffectsVolume;
            };
        }
    }

    private void Update() {
        if (_audioSource != null) {
            if (GameController.IsGamePaused && _audioSource.isPlaying) {
                _audioSource.Pause();
            }

            if (!GameController.IsGamePaused && !_audioSource.isPlaying) {
                _audioSource.UnPause();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        switch (transform.parent.tag) {
            case "Player":
                if (other.CompareTag("Enemy")) {
                    // Optimize
                    other.gameObject.GetComponent<EnemyController2D>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                else if (other.CompareTag("Boss1"))
                {
                    other.gameObject.GetComponent<Boss1Controller>().TakeDamage(damage);
                    Destroy(gameObject);
                } else if (other.CompareTag("Boss2")) {
                    other.gameObject.GetComponent<Boss2Controller>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                break;
            case "Enemy":
                if (other.CompareTag("Player")) {
                    other.gameObject.GetComponent<CharacterController2D>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                break;
            case "LaserPlatform":
                if (other.CompareTag("Player")) {
                    var playerController = other.gameObject.GetComponent<CharacterController2D>(); 
                    playerController.TakeDamage(playerController.Hp);
                }
                break;
            case "Boss1":
                if (other.CompareTag("Player")) {
                    other.gameObject.GetComponent<CharacterController2D>().TakeDamage(damage);
                    Destroy(gameObject);
                } else if (!other.CompareTag("Granica")) {
                    Destroy(gameObject);
                }
                break;
            case "Boss2":
                if (other.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<CharacterController2D>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                else if (other.CompareTag("Boss2")) {
                    break;
                }
                else if (!other.CompareTag("Granica"))
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        switch (transform.parent.tag) {
            case "ElectricPlatform":
                if (other.CompareTag("Player")) {
                    var playerController = other.gameObject.GetComponent<CharacterController2D>();
                    playerController.TakeDamage(damage);
                }
                break;
            
        }
        switch (transform.tag)
        {
            case "Elektrycznosc":
                if (other.CompareTag("Player"))
                {
                    var playerController = other.gameObject.GetComponent<CharacterController2D>();
                    playerController.TakeDamage(damage*2);
                }
                break;
        }
    }
}
