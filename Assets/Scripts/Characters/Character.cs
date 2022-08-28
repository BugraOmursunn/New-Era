using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageAble
{
	public abstract CharacterData CharData { get; set; }
	public abstract Animator CharAnimator { get; set; }
	public abstract float Health { get; set; }
	public abstract float Mana { get; set; }
	public abstract float Stamina { get; set; }

	public abstract bool IsDead { get; set; }

	public abstract void GetDamage(float damage);
}
