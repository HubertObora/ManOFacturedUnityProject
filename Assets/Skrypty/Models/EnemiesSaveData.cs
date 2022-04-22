using System;
using UnityEngine;

namespace Skrypty.Models {
  /// <summary>
  /// Reprezentuje dane przeciwnika dla zapisywania ich stanu w plik
  /// </summary>
  [Serializable]
  public class EnemySaveData {
    public float[] Position { get; set; }
    public float[] Rotation { get; set; }
    public float Speed { get; set; }
    public float GroundCheckDistance{ get; set; }
    public float PlayerCheckDistance{ get; set; }
    public int FireRate{ get; set; }
    public int CollisionDamage{ get; set; }
    public int CollisionDamageRate{ get; set; }
    public float Hp { get; set; }
    public bool isMovingRight{ get; set; }

    public EnemySaveData(Transform enemyTransform, EnemyController2D controller2D) {
      Position = new[] {enemyTransform.position.x, enemyTransform.position.y, enemyTransform.position.z};
      Rotation = new[]
          {enemyTransform.rotation.x, enemyTransform.rotation.y, enemyTransform.rotation.z, enemyTransform.rotation.w};
      Speed = controller2D.speed;
      GroundCheckDistance = controller2D.groundCheckDistance;
      PlayerCheckDistance = controller2D.playerCheckDistance;
      FireRate = controller2D.fireRate;
      CollisionDamage = controller2D.collisionDamage;
      CollisionDamageRate = controller2D.collisionDamageRate;
      Hp = controller2D.Hp;
      isMovingRight = controller2D.movingRight;
    }
  }
}