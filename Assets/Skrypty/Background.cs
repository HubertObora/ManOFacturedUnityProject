using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private float dlugosc, pozycjaStartX, pozycjaStartY;
    public GameObject cam;
    public float kameraEfekt;
    void Start()
    {
        pozycjaStartX = transform.position.x;
        pozycjaStartY = transform.position.y;
        dlugosc = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float dystans = (cam.transform.position.x * kameraEfekt);
        float dystans2 = (cam.transform.position.y * kameraEfekt);
        transform.position = new Vector3(pozycjaStartX + dystans, pozycjaStartY + dystans2, transform.position.z);
    }
}
