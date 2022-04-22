using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class bossFightStart : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private GameObject granica2;
    [SerializeField] private GameObject laser1;
    [SerializeField] private GameObject laser2;
    [SerializeField] private GameObject laser3;
    [SerializeField] private GameObject laser4;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        confiner.m_BoundingShape2D = granica2.GetComponent<PolygonCollider2D>();
        laser1.SetActive(true);
        laser2.SetActive(true);
        laser3.SetActive(true);
        laser4.SetActive(true);
    }
}
