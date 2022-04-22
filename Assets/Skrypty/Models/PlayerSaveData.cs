using System;
using UnityEngine;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje dane gracza dla zapisywania ich stanu w plik
  /// </summary>
  [Serializable]
  public class PlayerSaveData {
    public float[] Position { get; set; }
    public float[] Scale { get; set; }
    public float JumpForce { get; set; }
    public float CrouchSpeed { get; set; }
    public float MovementSmoothing { get; set; }
    public int FireRate { get; set; }
    public int AmmoPerBattery { get; set; }
    public int Ammo { get; set; }
    public int Fuel { get; set; }
    public float JetpackForce { get; set; }
    public int FlyRate { get; set; }
    public float Hp { get; set; }
    public int MaxHp { get; set; }
    public string Laser { get; set; }
    public bool IsSpringCollected { get; set; }
    public bool IsJetPackCollected { get; set; }
    public bool CanFly { get; set; }
    public bool IsRedLaserPicked { get; set; }
    public bool IsGreenLaserPicked { get; set; }
    public int AmmoPerShoot { get; set; }

    public PlayerSaveData(Transform playerTransform, CharacterController2D controller2D) {
      Position = new[] {playerTransform.position.x, playerTransform.position.y, playerTransform.position.z};
      Scale = new[] {playerTransform.localScale.x, playerTransform.localScale.y, playerTransform.localScale.z};
      JumpForce = controller2D.jumpForce;
      MovementSmoothing = controller2D.movementSmoothing;
      FireRate = controller2D.fireRate;
      AmmoPerBattery = controller2D.ammoPerBattery;
      Ammo = CharacterController2D.Ammo;
      Fuel = controller2D.fuel;
      JetpackForce = controller2D.jetpackForce;
      FlyRate = controller2D.flyRate;
      Hp = controller2D.Hp;
      MaxHp = CharacterController2D.MaxHealth;
      Laser = controller2D.laser.name;
      IsSpringCollected = controller2D.canJump;
      IsJetPackCollected = controller2D.IsJetpackPicked;
      CanFly = controller2D.canFly;
      IsRedLaserPicked = controller2D.IsRedLaserPicked;
      IsGreenLaserPicked = controller2D.IsGreenLaserPicked;
      AmmoPerShoot = controller2D.ammoPerShoot;
    }
  }
}