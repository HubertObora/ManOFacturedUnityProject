using UnityEngine;

/// <summary>
/// Klasa, obsługująca za przesuwanie gracza na taśmie
/// </summary>
public class Tasma : MonoBehaviour
{
    private bool collided;
    Rigidbody2D player;
    ConstantForce2D c2d;
    public float push = 1f;
    
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        c2d = GetComponent<ConstantForce2D>();
        c2d.force = new Vector2(push, 0);
        c2d.enabled = false;
    }
    
    void FixedUpdate()
    {
        if (collided)
        {
            c2d.enabled = true;
        }
        else
        {
            c2d.enabled = false;
        }
            
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tasma"))
        {
            collided = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tasma"))
        {
            collided = false;
        }
    }

}

