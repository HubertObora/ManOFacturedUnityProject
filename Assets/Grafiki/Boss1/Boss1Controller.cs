using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Boss1Controller : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;
    public Animator animator;
    float attackDelay;
    float animationDelay;
    string whatAnimation;
    [SerializeField] private GameObject granica2;
    public GameObject player;
    private Transform bossPosition;
    public GameObject gate1;
    public GameObject gate2;
    private int collisionDamage = 15;
    private int collisionDamageRate = 2;
    private float _nextTimeToCollisionDamage = 0f;
    private float timeOfDeathAnimation = 4f;
    [SerializeField] private GameObject leftTopWeapon;
    [SerializeField] private GameObject leftMiddleWeapon;
    [SerializeField] private GameObject leftBottomWeapon;
    [SerializeField] private GameObject rightTopWeapon;
    [SerializeField] private GameObject rightMiddleWeapon;
    [SerializeField] private GameObject rightBottomWeapon;
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject[] electricElements;
    private float _nextTimeToFire = 0f;
    public int fireRate = 2;
    public static int MaxHealth { get; private set; } = 300;
    public float Hp { get; set; } = MaxHealth;
    public delegate void HPHandler(float value);
    public event HPHandler OnDamageTaken;

    // Start is called before the first frame update
    void Start()
    {
        bossPosition = GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        //for tests
       // if (Input.GetKeyDown(KeyCode.F)) {
            //Shoot(leftMiddleWeapon.transform, Vector2.left);
       // }
    }

    private void FixedUpdate() {
        var playerLeftTopInfo = Physics2D.Raycast(leftTopWeapon.transform.position, Vector2.left * 10, 10);
        var playerLeftMiddleInfo = Physics2D.Raycast(leftMiddleWeapon.transform.position, Vector2.left * 10, 10);
        var playerLeftBottomInfo = Physics2D.Raycast(leftBottomWeapon.transform.position, Vector2.left * 10, 10);
        var playerRightTopInfo = Physics2D.Raycast(rightTopWeapon.transform.position, Vector2.right * 10, 10);
        var playerRightMiddleInfo = Physics2D.Raycast(rightMiddleWeapon.transform.position, Vector2.right * 10, 10);
        var playerRightBottomInfo = Physics2D.Raycast(rightBottomWeapon.transform.position, Vector2.right * 10, 10);
        if (playerLeftTopInfo.collider != null) {
            if (playerLeftTopInfo.collider.CompareTag("Player") && Time.time >= _nextTimeToFire) {
                attackDelay = 0.2f;
                whatAnimation = "leftTopAttack";
                animator.SetBool(whatAnimation, true);
                StartCoroutine(Shoot(leftTopWeapon.transform, Vector2.left, attackDelay, whatAnimation));
                _nextTimeToFire = Time.time + 3f / fireRate;
            }
        }

        if (playerLeftMiddleInfo.collider != null) {
            if (playerLeftMiddleInfo.collider.CompareTag("Player") && Time.time >= _nextTimeToFire) {
                attackDelay = 1f;
                whatAnimation = "leftMidAttack";
                animator.SetBool(whatAnimation, true);
                StartCoroutine(Shoot(leftMiddleWeapon.transform, Vector2.left,attackDelay, whatAnimation));
                _nextTimeToFire = Time.time + 3f / fireRate;
            }
        }


        if (playerLeftBottomInfo.collider != null) {
            if (playerLeftBottomInfo.collider.CompareTag("Player")&& animator.GetBool("botAttack") == false && Time.time >= _nextTimeToFire) {
                whatAnimation = "botAttack";
                animator.SetBool(whatAnimation, true);
                StartCoroutine(EnableElectricity(whatAnimation));
                _nextTimeToFire = Time.time + 3f / fireRate;
            }	
        }

        if (playerRightTopInfo.collider != null) {
            if (playerRightTopInfo.collider.CompareTag("Player") && Time.time >= _nextTimeToFire) {
                attackDelay = 0.5f;
                whatAnimation = "rightTopAttack";
                animator.SetBool(whatAnimation, true);
                StartCoroutine(Shoot(rightTopWeapon.transform, Vector2.right,attackDelay, whatAnimation));
                _nextTimeToFire = Time.time + 3f / fireRate;
            }	
        }

        if (playerRightMiddleInfo.collider != null) {
            if (playerRightMiddleInfo.collider.CompareTag("Player") && Time.time >= _nextTimeToFire) {
                attackDelay = 0.4f;
                whatAnimation = "rightMidAttack";
                animator.SetBool(whatAnimation, true);
                StartCoroutine(Shoot(rightMiddleWeapon.transform, Vector2.right,attackDelay, whatAnimation));
                _nextTimeToFire = Time.time + 3f / fireRate;
            }	
        }

        if (playerRightBottomInfo.collider != null) {
            if (playerRightBottomInfo.collider.CompareTag("Player") && animator.GetBool("botAttack") == false && Time.time >= _nextTimeToFire)
            {
                whatAnimation = "botAttack";
                animator.SetBool(whatAnimation, true);
                StartCoroutine(EnableElectricity(whatAnimation));
                _nextTimeToFire = Time.time + 3f / fireRate;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Time.time >= _nextTimeToCollisionDamage)
        {
            other.gameObject.GetComponent<CharacterController2D>().TakeDamage(collisionDamage);
            _nextTimeToCollisionDamage = Time.time + 1f / collisionDamageRate;
        }
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
        }
    }
    private void Dead()
    {
        foreach (var element in electricElements)
        {
            Destroy(element);
        }
        animator.SetTrigger("robotDie");
        Destroy(gameObject, timeOfDeathAnimation);
        gate1.active = false;
        gate2.active = false;
        leftBottomWeapon.active = false;
        leftMiddleWeapon.active = false;
        leftTopWeapon.active = false;
        rightBottomWeapon.active = false;
        rightMiddleWeapon.active = false;
        rightTopWeapon.active = false;
        confiner.m_BoundingShape2D= granica2.GetComponent<PolygonCollider2D>();
    }

    IEnumerator EnableElectricity(string whatAnimation) {
        yield return new WaitForSeconds(1);
        foreach (var element in electricElements) {
            element.SetActive(true);
            StartCoroutine(DisableElectricity(whatAnimation));
        }
    }

    IEnumerator DisableElectricity(string whatAnimation) {
        yield return new WaitForSeconds(1);
        animator.SetBool(whatAnimation, false);
        foreach (var element in electricElements) {
            element.SetActive(false);
        }
    }
    IEnumerator Animation(float animationDelay)
    {
        yield return new WaitForSeconds(animationDelay);
    }
    IEnumerator Shoot(Transform weapon, Vector2 direction, float attackDelay, string whatAnimation) {
        yield return new WaitForSeconds(attackDelay);
        var laserBullet = Instantiate(laser, weapon.position, new Quaternion(0, 0, 0, 0), transform).GetComponent<Rigidbody2D>();
        laserBullet.AddForce(direction * 0.025f);
        animator.SetBool(whatAnimation, false);
        Destroy(laserBullet.gameObject, 1f);
    }
}
