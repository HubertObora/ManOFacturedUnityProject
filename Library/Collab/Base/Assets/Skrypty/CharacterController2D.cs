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
	public static Rigidbody2D m_Rigidbody2D;
	public float jumpForce = 270f;
	public bool canJump;
	public float movementSmoothing = .2f; // czas przejscia do pelnej szybkosci ruchu
	private Vector3 currentVelocity = Vector3.zero; // poczatkowy wektor gracza przy przechodzenia do pelnej predkosci
	private bool grounded;
	private bool facingRight = true;
	[SerializeField] private LayerMask whatIsGround;							
	[SerializeField] private Transform groundCheck; //groundcheck przypisany do postaci gracza
	//zmienne do broni
	private bool canShoot;
	private bool canShoot2;
	[SerializeField] private Transform weapon;
	[SerializeField] private GameObject laser;
	public int fireRate = 2;
	public int ammoPerBattery = 5;
	//zmienna do jetpacka
	public bool canFly;
	public int fuel = 100;
	public float jetpackForce = 80f;
	public int flyRate = 2;
	private float _nextTimeToFire = 0f;
	private float _nextTimeToFly = 0f;
	public Animator animator;
	public bool IsJetpackPicked { get; private set; } = false;
	public static int MaxHealth { get; private set; } = 100;
	public float Hp { get; private set; } = MaxHealth;
	public static int MaxAmmo { get; private set; } = 50;
	public int Ammo { get; private set; } = 5;
	
	public delegate void HPHandler(float value);
	public delegate void ItemsCollisionHandler(PickUpItemsEnum type, int value);

	public delegate void ShootHandler(int value);
	
	public event HPHandler OnDamageTaken;
	public event ItemsCollisionHandler OnItemCollected;
	public event ShootHandler OnShoot;
	public EnfOfLevel endoflevel;
	private string sceneName;
	
	private AudioSource _audioSource;
	[SerializeField] private AudioClip gearCollectedSound;
	[SerializeField] private AudioClip playerJumpedSound;

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
			canShoot = false;
			canShoot2 = false;
		}
		else if (sceneName == "Level 2")
		{
			canJump = true;
			IsJetpackPicked = false;
			canShoot = false;
			canShoot2 = false;
		}
		else if (sceneName == "Level 3")
		{
			canJump = true;
			IsJetpackPicked = false;
			canShoot = true;
			canShoot2 = false;
		}
		else if (sceneName == "Level 4")
		{
			canJump = true;
			IsJetpackPicked = false;
			canShoot = true;
			canShoot2 = true;
		}
		else if (sceneName == "Level 5")
		{
			canJump = true;
			IsJetpackPicked = true;
			canShoot = true;
			canShoot2 = true;
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
	}

	private void Update() {
		if (Input.GetButton("Fire1") && Time.time >= _nextTimeToFire && canShoot) {
			if (Ammo > 0) { 
				Shoot(1);
				_nextTimeToFire = Time.time + 1f / fireRate;
			}
		}
		if (Input.GetButton("Fire1") && Time.time >= _nextTimeToFire && canShoot2)
		{
			if (Ammo > 0)
			{
				Shoot(2);
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
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, .2f, whatIsGround); // towrzy tablice obiektow wokol gracza 
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

	private void Shoot(int ammo) {
		animator.SetBool("czyStrzal", true);
		var laserBullet = Instantiate(laser, weapon.position, new Quaternion(0, 0, 0, 0), transform).GetComponent<Rigidbody2D>();
		laserBullet.AddForce(transform.localScale.x > 0 ? Vector2.right * 0.1f : Vector2.left * 0.1f);
			
		Destroy(laserBullet.gameObject, 0.2f);
		Ammo-=ammo;
		if (OnShoot != null) OnShoot(-1);
	}
	
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
					canShoot = true;
					endoflevel.InfoSprezyna.SetActive(true);
					Time.timeScale = 0f;
					Destroy(other.gameObject);
					break;
                }
			case "Pistolet2":
				{
					canShoot2 = true;
					endoflevel.InfoSprezyna.SetActive(true);
					Time.timeScale = 0f;
					Destroy(other.gameObject);
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
	IEnumerator AmmoRefill(Collider2D bateria)
	{
		yield return new WaitForSeconds(5f);
		bateria.gameObject.active = true;
	}
	private void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale; // pobiera aktualny obrot gracza z transform scale
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	IEnumerator Dead() {
		animator.SetTrigger("smierc");
		yield return new WaitForSeconds(0.5f);
		Time.timeScale = 0f;
		endoflevel.InfoDead.SetActive(true);
		//gameObject.SetActive(false);
	}

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
	}

	public void ChangeLaser(GameObject laser) {
		this.laser = laser;
	}

	public void EnableJetPack() {
		canJump = false;
		canFly = true;
	}

	public void EnableJump() {
		canFly = false;
		canJump = true;
	}
}
