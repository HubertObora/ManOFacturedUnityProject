using System;
using System.Collections;
using Skrypty.Enums;
using Skrypty.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
	//zmienne do ruchu
	private bool grounded;
	private bool facingRight = true;
	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private Transform groundCheck; //groundcheck przypisany do postaci gracza
	private Vector3 currentVelocity = Vector3.zero; // poczatkowy wektor gracza przy przechodzenia do pelnej predkosci
	public static Rigidbody2D m_Rigidbody2D;
	public float jumpForce = 270f;
	public bool canJump;
	public float movementSmoothing = .2f; // czas przejscia do pelnej szybkosci ruchu
	//zmienne do broni
	[SerializeField] private int ammoPerRedLaserShoot = 1;
	[SerializeField] private int ammoPerGreenLaserShoot = 2;
	[SerializeField] private Transform weapon;
	[SerializeField] private GameObject greenLaser;
	[SerializeField] private GameObject redLaser;
	private float _nextTimeToFire = 0f;
	private float _nextTimeToFly = 0f;
	public bool IsRedLaserPicked;
	public bool IsGreenLaserPicked;
	public int ammoPerShoot;
	public GameObject laser;
	public int fireRate = 2;
	public int ammoPerBattery = 5;
	//zmienna do jetpacka
	public bool canFly;
	public int fuel = 100;
	public float jetpackForce = 80f;
	public int flyRate = 2;
	public Animator animator;
	public bool IsJetpackPicked { get; private set; } = false;
	public static int MaxHealth { get; private set; } = 100;
	public float Hp { get; private set; } = MaxHealth;
	public int hpPerGear = 2;
	public static int MaxAmmo { get; private set; } = 50;
	public static int Ammo { get; private set; } = 0;

	public delegate void HPHandler(float value);
	public delegate void ItemsCollisionHandler(PickUpItemsEnum type, int value);

	public delegate void ShootHandler(int value);
	
	public event HPHandler OnDamageTaken;
	public event ItemsCollisionHandler OnItemCollected;
	public event ShootHandler OnShoot;
	public event Action OnRedLaserPicked;
	public event Action OnGreenLaserPicked;
	public EnfOfLevel endoflevel;
	private string sceneName;
	
	private AudioSource _audioSource;
	[SerializeField] private AudioClip gearCollectedSound;
	[SerializeField] private AudioClip playerJumpedSound;
	[SerializeField] private AudioClip shootSound;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	private void Awake()
	{ 
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		Scene currentScene = SceneManager.GetActiveScene();
		sceneName = currentScene.name;
		if (sceneName == "Level 1")
		{
			canJump = false;
			IsJetpackPicked = false;
			IsRedLaserPicked = false;
			IsGreenLaserPicked = false;
		}
		else if (sceneName == "Level 2")
		{
			canJump = true;
			IsJetpackPicked = false;
			IsRedLaserPicked = false;
			IsGreenLaserPicked = false;
		}
		else if (sceneName == "Level 3")
		{
			canJump = true;
			IsJetpackPicked = false;
			IsRedLaserPicked = true;
			IsGreenLaserPicked = false;
		}
		else if (sceneName == "Level 4")
		{
			canJump = true;
			IsJetpackPicked = false;
			IsRedLaserPicked = true;
			IsGreenLaserPicked = true;
		}
		else if (sceneName == "Level 5")
		{
			canJump = true;
			IsJetpackPicked = true;
			IsRedLaserPicked = true;
			IsGreenLaserPicked = true;
		}
		UpgradesShop.OnHpUpgradeBuy += lvl => {
			MaxHealth += lvl * 10;
			Debug.Log($"Max hp increased. New max hp = {MaxHealth}");
		};

		UpgradesShop.OnJetPackUpgradeBuy += lvl => {
			fuel += lvl * 100;
			Debug.Log($"Fuel added. New fuel {fuel}");
		};
	}

	private void Start() {
		_audioSource = GetComponent<AudioSource>();

		if (_audioSource != null) {
			_audioSource.volume = SettingsManager.gameEffectsVolume;
		}

		SettingsManager.OnSettingsSaved += () => {
			_audioSource.volume = SettingsManager.gameEffectsVolume;
		};
		
		SetAmmoPerShoot(laser);
	}

	private void Update() {
		if (Input.GetButton("Fire1") && Time.time >= _nextTimeToFire) {
			if (Ammo > 0) { 
				Shoot();
				_nextTimeToFire = Time.time + 1f / fireRate;
			}
		}
		if (Input.GetButton("Jump") && Time.time >= _nextTimeToFly && fuel > 0 && canFly && IsJetpackPicked)
		{
			m_Rigidbody2D.AddForce(new Vector2(0f, jetpackForce));
			fuel -= 1;
			_nextTimeToFly = Time.time + 1f / flyRate;
			Debug.Log(fuel);
		}
		
		if (GameController.IsGamePaused && _audioSource.isPlaying) {
			_audioSource.Pause();
		}

		if (!GameController.IsGamePaused && !_audioSource.isPlaying) {
			_audioSource.UnPause();
		}
	}

	private void FixedUpdate()
	{
		grounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, .2f, whatIsGround); // tworzy tablice obiektow wokol ground checka gracza 
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.tag!= "Granica" && colliders[i].gameObject != gameObject && !colliders[i].gameObject.transform.IsChildOf(gameObject.transform))
			{
				grounded = true;
			}
		}
		animator.SetBool("czyStrzal", false);
		animator.SetBool("grounded", grounded);
	}

	/// <summary>
	/// Wypuszcza laser, którym nanosi uderzenia przeciwnikom
	/// </summary>
	private void Shoot() {
		animator.SetBool("czyStrzal", true);
		var laserBullet = Instantiate(laser, weapon.position, new Quaternion(0, 0, 0, 0), transform).GetComponent<Rigidbody2D>();
		laserBullet.AddForce(transform.localScale.x > 0 ? Vector2.right * 0.1f : Vector2.left * 0.1f);

		if (laser.name.ToLower().Contains("laserred")) {
			Destroy(laserBullet.gameObject, 0.3f);
		} else if (laser.name.ToLower().Contains("lasergreen")) {
			Destroy(laserBullet.gameObject, 0.6f);
		}
		Ammo-=ammoPerShoot;
		if (OnShoot != null) OnShoot(-ammoPerShoot);
		if (shootSound != null) _audioSource.PlayOneShot(shootSound);
		Debug.Log(ammoPerShoot);
	}
	
	/// <summary>
	/// Przemieszcza gracza w przestrzeni
	/// </summary>
	/// <param name="move">Wskazuje kierunek i silę ruchu gracza</param>
	/// <param name="jump">Wskazuje czy gracz użył skoku</param>
	public void Move(float move, bool jump)
	{
		Vector2 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y); //wektor predkosci
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref currentVelocity, movementSmoothing); // plynne przejscie do ruchu
		if (move > 0 && !facingRight)
		{
			Flip();
		}
		else if (move < 0 && facingRight)
		{
			Flip();
		}
		if (grounded && jump && canJump)
		{
			grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			animator.SetBool("czySkok", false);

			if (_audioSource != null) {
				if (playerJumpedSound != null) {
					_audioSource.PlayOneShot(playerJumpedSound);
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		switch (other.tag) {
			case "Pistolet":
                {
					IsRedLaserPicked = true;
					endoflevel.InfoSprezyna.SetActive(true);
					Time.timeScale = 0f;
					Destroy(other.gameObject);
					if (OnRedLaserPicked != null) OnRedLaserPicked.Invoke();
					break;
                }
			case "Pistolet2":
				{
					IsGreenLaserPicked = true;
					endoflevel.InfoSprezyna.SetActive(true);
					Time.timeScale = 0f;
					Destroy(other.gameObject);
					if (OnGreenLaserPicked != null) OnGreenLaserPicked.Invoke();
					break;
				}
			case "Sprezyna":
				{
					canJump = true;
					endoflevel.InfoSprezyna.SetActive(true);
					Time.timeScale = 0f;
					Destroy(other.gameObject);
					break;
				}
			case "Jetpack":
				{
					IsJetpackPicked = true;
					endoflevel.InfoSprezyna.SetActive(true);
					Time.timeScale = 0f;
					Destroy(other.gameObject);
					break;
				}
			case "Zebatka":
				OnItemCollected(PickUpItemsEnum.Gears, 1);
				var hpToAdd = Hp + hpPerGear <= MaxHealth ? hpPerGear : MaxHealth - Hp;
				if (OnDamageTaken != null) OnDamageTaken(-hpToAdd);
				Hp += hpToAdd;

				if (gearCollectedSound != null) {
					_audioSource.PlayOneShot(gearCollectedSound);	
				}
				
				Destroy(other.gameObject);
				break;
			case "Bateria":
				if (Ammo < MaxAmmo) {
					var ammoToAdd = Ammo + ammoPerBattery <= MaxAmmo ? ammoPerBattery : MaxAmmo - Ammo;
					OnItemCollected(PickUpItemsEnum.Battery, ammoToAdd);
					Ammo += ammoToAdd;
					if(sceneName == "Level 3" || sceneName == "Level 5")
                    {
						other.gameObject.active = false;
						StartCoroutine(AmmoRefill(other));
					}
                    else
                    {
						Destroy(other.gameObject);
                    }
					break;
				}
				break;
		}
	}

	/// <summary>
	/// Ustawia ilość amunicji, która będzie odejmowania za jeden strzał w zależności od użytego lasera
	/// </summary>
	/// <param name="laserObj">Obiekt lasera, którym strzelił gracz</param>
	private void SetAmmoPerShoot(GameObject laserObj) {
		if (laserObj.name.ToLower().Contains("laserred")) {
			ammoPerShoot = ammoPerRedLaserShoot;
		} else if (laserObj.name.ToLower().Contains("lasergreen")) {
			ammoPerShoot = ammoPerGreenLaserShoot;
		}
	} 
	
	IEnumerator AmmoRefill(Collider2D bateria)
	{
		yield return new WaitForSeconds(5f);
		bateria.gameObject.SetActive(true);
	}
	
	/// <summary>
	/// Obraca gracza w przeciwną stronę
	/// </summary>
	private void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale; // pobiera aktualny obrot gracza z transform scale
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	/// <summary>
	/// Odpowiada za śmierć gracza
	/// </summary>
	/// <returns>IEnumerator z opóźnieniem o 0.5s</returns>
	IEnumerator Dead() {
		animator.SetTrigger("smierc");
		yield return new WaitForSeconds(0.5f);
		Time.timeScale = 0f;
		endoflevel.InfoDead.SetActive(true);
		//gameObject.SetActive(false);
	}

	/// <summary>
	/// Odpowiada za otrzymanie uderzeń od przeciwników
	/// </summary>
	/// <param name="damage">Ilość otrzymanego obrażenia</param>
	public void TakeDamage(float damage) {
		if (OnDamageTaken != null) OnDamageTaken(damage);
		if ((Hp - damage) > 0) {
			Hp -= damage;
		} else {
			m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
			Hp = 0;
			StartCoroutine(Dead());
		}
	}

	/// <summary>
	/// Zastosowuje zmiany zapisane w pliku ze stanem gry przy kontynuacji gry
	/// </summary>
	/// <param name="saveData">Obiekt z informacjami o zapisanym stanie gracza</param>
	public void ApplySavedData(PlayerSaveData saveData) {
		jumpForce = saveData.JumpForce;
		movementSmoothing = saveData.MovementSmoothing;
		fireRate = saveData.FireRate;
		ammoPerBattery = saveData.AmmoPerBattery;
		Ammo = saveData.Ammo;
		fuel = saveData.Fuel;
		jetpackForce = saveData.JetpackForce;
		Hp = saveData.Hp;
		MaxHealth = saveData.MaxHp;
		canJump = saveData.IsSpringCollected;
		IsJetpackPicked = saveData.IsJetPackCollected;
		canFly = saveData.CanFly;
		IsRedLaserPicked = saveData.IsRedLaserPicked;
		IsGreenLaserPicked = saveData.IsGreenLaserPicked;
		ammoPerShoot = saveData.AmmoPerShoot;

		switch (saveData.Laser) {
			case "LaserRed":
				laser = redLaser;
				break;
			case "LaserGreen":
				laser = greenLaser;
				break;
			default:
				laser = redLaser;
				break;
		}
	}

	/// <summary>
	/// Zmienia laser, którym strzela gracz
	/// </summary>
	/// <param name="laserObj">Obiekt lasera, którego ma używać gracz</param>
	public void ChangeLaser(GameObject laserObj) {
		laser = laserObj;
		SetAmmoPerShoot(laserObj);
	}

	/// <summary>
	/// Aktywuje JetPack
	/// </summary>
	public void EnableJetPack() {
		canJump = false;
		canFly = true;
	}

	/// <summary>
	/// Aktywuje skok
	/// </summary>
	public void EnableJump() {
		canFly = false;
		canJump = true;
	}
}
