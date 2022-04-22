using System.Collections;
using UnityEngine;

/// <summary>
/// Klasa, obsługująca poruszanie gracz
/// </summary>
public class PlayerMovment : MonoBehaviour
{
    private GameObject teleport;
    private bool canTeleport = true;
    private float horizontalMove = 0f;
    private float runSpeed = 15f;
    private bool jump = false;
    private Vector3 checkpoint;
    public CharacterController2D controller;
    public Animator animator;
    public GameObject target;
    public FallCheck fallCheck;
   
    void Start()
    {
        checkpoint = transform.position;
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if(horizontalMove !=0)
        {
            animator.SetBool("czyBieg", true);
        }
        else
        {
            animator.SetBool("czyBieg", false);
        }
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("czySkok", true);
        }
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
    
    IEnumerator zmienFallCheck()
    {
        yield return new WaitForSeconds(0.5f);
        fallCheck.protect = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("FallColider"))
        {
            fallCheck.protect = true;
            StartCoroutine(zmienFallCheck());
            transform.position = checkpoint;
        }
        else if (collision.CompareTag("CheckpointCollider"))
        {
            checkpoint = transform.position;
        }
        else if (collision.CompareTag("Teleport"))
        {
            teleport = collision.gameObject;
            if(canTeleport)
            {
                StartCoroutine(Teleportuj());
                canTeleport = false;
                StartCoroutine(WaitForNextTep());
            }
        }
    }

    IEnumerator Teleportuj()
    {
        yield return new WaitForSeconds(1);
        transform.position = teleport.GetComponent<Teleport>().CelTeleportacji().position;
    }
    IEnumerator WaitForNextTep()
    {
        yield return new WaitForSeconds(3);
        canTeleport = true;
    }
}
