using UnityEngine;

/// <summary>
/// Klasa, obsługująca teleportację gracza
/// </summary>
public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform celTeleportacji;
    public Transform CelTeleportacji()
    {
        return celTeleportacji;
    }
}
