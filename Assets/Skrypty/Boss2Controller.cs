using Skrypty.Models;
using UnityEngine;
using Cinemachine;

public class Boss2Controller : MonoBehaviour
{
	public float speed;
	public float timeOfDeathAnimation = 3.5f;
	private float playerCheckDistance = 40f;
	[SerializeField] private Transform playerCheck;
	[SerializeField] private Transform playerCheck2;
	[SerializeField] private GameObject laser;
	public int fireRate = 2;
	public int collisionDamage = 15;
	public int collisionDamageRate = 2;
	public Animator animator;
	[SerializeField] private CinemachineConfiner confiner;
	[SerializeField] private GameObject granica3;
	[SerializeField] private GameObject laser1;
	[SerializeField] private GameObject laser2;
	[SerializeField] private GameObject laser3;
	[SerializeField] private GameObject laser4;
	public static int MaxHealth { get; private set; } = 100;
	public delegate void HPHandler(float value);
	public event HPHandler OnDamageTaken;
	public float Hp { get; set; } = MaxHealth;
	bool isdead;

	public bool movingRight = true;
	private float _nextTimeToFire = 0f;
	private float _nextTimeToCollisionDamage = 0f;
    private void Update()
	{
		Debug.DrawRay(playerCheck.position, Vector3.right * 10, Color.green);
		Debug.DrawRay(playerCheck2.position, Vector3.left * 10, Color.green);
	}

	private void FixedUpdate()
	{
		if (movingRight) {
			if(!isdead)
            {
				transform.Translate(Vector3.right * speed);
			}
		} else {	
			if (!isdead)
			{
				transform.Translate(Vector3.left * speed);
			}
		}

		var playerInfo = Physics2D.Raycast(playerCheck.position, Vector2.right * 10, playerCheckDistance);
		var playerInfo2 = Physics2D.Raycast(playerCheck2.position, Vector2.left * 10, playerCheckDistance);
		if (transform.position.x >= -0.1f)
		{
			movingRight = false;
		}

		if (transform.position.x <= -10.8f)
		{
			movingRight = true;
		}

		if (playerInfo.collider != null)
		{
			if (playerInfo.collider.CompareTag("Player") && Time.time >= _nextTimeToFire && !isdead)
			{
				Shoot(playerCheck, Vector2.right);
				_nextTimeToFire = Time.time + 3f / fireRate;
			}
		}
		if(playerInfo2.collider != null)
        {
			if (playerInfo2.collider.CompareTag("Player") && Time.time >= _nextTimeToFire && !isdead)
			{
				Shoot(playerCheck2, Vector2.left);
				_nextTimeToFire = Time.time + 2f / fireRate;
			}
		}
	}

	private void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Player") && Time.time >= _nextTimeToCollisionDamage)
		{
			other.gameObject.GetComponent<CharacterController2D>().TakeDamage(collisionDamage);
			_nextTimeToCollisionDamage = Time.time + 2f / collisionDamageRate;
		}
	}

	private void Shoot(Transform Check, Vector2 direction)
	{
		var laserBullet = Instantiate(laser, Check.position, new Quaternion(0, 0, 0, 0), transform).GetComponent<Rigidbody2D>();

		laserBullet.AddForce(direction * 0.03f);

		Destroy(laserBullet.gameObject, 0.7f);
	}

	private void Dead()
	{
		isdead = true;
		animator.SetTrigger("die");
		Destroy(gameObject, timeOfDeathAnimation);
		laser1.SetActive(false);
		laser2.SetActive(false);
		laser3.SetActive(false);
		laser4.SetActive(false);
	}
	
	public void TakeDamage(float damage)
	{
		if (OnDamageTaken != null) OnDamageTaken(damage);
		if ((Hp - damage) > 0)
		{
			Hp -= damage;
		}
		else
		{
			Hp = 0;
			Dead();
			confiner.m_BoundingShape2D = granica3.GetComponent<PolygonCollider2D>();
		}
	}

	public void ApplySavedData(EnemySaveData saveData)
	{
		speed = saveData.Speed;
		playerCheckDistance = saveData.PlayerCheckDistance;
		fireRate = saveData.FireRate;
		collisionDamage = saveData.CollisionDamage;
		collisionDamageRate = saveData.CollisionDamageRate;
		Hp = saveData.Hp;
		movingRight = saveData.isMovingRight;
	}
}