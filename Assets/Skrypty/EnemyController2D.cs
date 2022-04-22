using Skrypty.Models;
using UnityEngine;

/// <summary>
/// Klasa, odpowiadająca za obsługę przeciwników
/// </summary>
public class EnemyController2D : MonoBehaviour {
	public float speed;
	public float timeOfDeathAnimation;
	public float groundCheckDistance = 1f;
	public float playerCheckDistance = 7f;
	public int fireRate = 2;
	public int collisionDamage = 15;
	public int collisionDamageRate = 2;
	public Animator animator;
	public float Hp { get; set; } = 20;
	public bool movingRight = true;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Transform playerCheck;
	[SerializeField] private GameObject laser;
	private float _nextTimeToFire = 0f;
	private float _nextTimeToCollisionDamage = 0f;

	private void Update() {
		Debug.DrawRay(playerCheck.position, movingRight ? Vector3.right * 10 : Vector3.left * 10, Color.green);
	}

	private void FixedUpdate() {

		transform.Translate(Vector3.right * speed);
		RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance);

		var playerInfo = Physics2D.Raycast(playerCheck.position, movingRight ? Vector2.right * 10 : Vector2.left * 10, playerCheckDistance);

		if (groundInfo.collider == false) {
			if (movingRight) {
				transform.eulerAngles = new Vector3(0, -180, 0);
				movingRight = false;
			} else {
				transform.eulerAngles = new Vector3(0, 0, 0);
				movingRight = true;
			}
		}

		if (playerInfo.collider != null) {
			if (playerInfo.collider.CompareTag("Player") && Time.time >= _nextTimeToFire) {
				Shoot();
				_nextTimeToFire = Time.time + 3f / fireRate;
			}	
		}
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player") && Time.time >= _nextTimeToCollisionDamage) {
			other.gameObject.GetComponent<CharacterController2D>().TakeDamage(collisionDamage);
			_nextTimeToCollisionDamage = Time.time + 2f / collisionDamageRate;
		}
	}

	/// <summary>
	/// Wypuszcza laser, którym nanosi uderzenia graczu
	/// </summary>
	private void Shoot() {
		var laserBullet = Instantiate(laser, playerCheck.position, new Quaternion(0, 0, 0, 0), transform).GetComponent<Rigidbody2D>();
		laserBullet.AddForce(movingRight ? Vector2.right * 0.2f : Vector2.left * 0.2f);
		Destroy(laserBullet.gameObject, 0.2f);
	}
	
	/// <summary>
	/// Odpowiada za śmierć przeciwnika
	/// </summary>
	private void Dead() {
		animator.SetBool("isDead", true);
		Destroy(gameObject, timeOfDeathAnimation);
	}
	
	/// <summary>
	/// Odpowiada za otrzymanie uderzeń od gracza
	/// </summary>
	/// <param name="damage"></param>
	public void TakeDamage(float damage) {
		if ((Hp - damage) > 0) {
			Hp -= damage;
		} else {
			Hp = 0;
			Dead();
		}
	}

	/// <summary>
	/// Zastosowuje zmiany zapisane w pliku ze stanem gry przy kontynuacji gry
	/// </summary>
	/// <param name="saveData">Obiekt z informacjami o zapisanym stanie przeciwnika</param>
	public void ApplySavedData(EnemySaveData saveData) {
		speed = saveData.Speed;
		groundCheckDistance = saveData.GroundCheckDistance;
		playerCheckDistance = saveData.PlayerCheckDistance;
		fireRate = saveData.FireRate;
		collisionDamage = saveData.CollisionDamage;
		collisionDamageRate = saveData.CollisionDamageRate;
		Hp = saveData.Hp;
		movingRight = saveData.isMovingRight;
	}
}
