using UnityEngine;

/// <summary>
/// Klasa, odpowiadająca za otrzymanie graczem obrażeń od spadku
/// </summary>
public class FallCheck : MonoBehaviour {
	private float _fallDamageModifier = 3;

	public float FallDamageModifier
	{
		get => _fallDamageModifier;
		set
		{
			if (value > 0) {
				_fallDamageModifier = value;
			}
		}
	}

	public CharacterController2D controller;
	public bool protect = false;

	private void Start() {
		FallDamageModifier = UpgradesShop.FallUpgradeLvl * 0.25f;
		
		UpgradesShop.OnFallUpgradeBuy += lvl => {
			FallDamageModifier -= lvl * 0.25f;
		};
	}

	void OnTriggerEnter2D(Collider2D col)
    {
		if (CharacterController2D.m_Rigidbody2D.velocity.y < -7 && !col.CompareTag("Zebatka")&&!col.CompareTag("Bateria") && !protect && !controller.canFly)
		{
			int damage = (int)CharacterController2D.m_Rigidbody2D.velocity.y;
			controller.TakeDamage(-damage * FallDamageModifier);
		}
	}
}
