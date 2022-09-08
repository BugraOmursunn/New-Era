using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageAble
{
	public abstract CharacterData CharacterData { get; set; }
	public abstract Animator CharAnimator { get; set; }

	public abstract bool IsDead { get; set; }
	public abstract void GetDamage(float damage);
	public abstract void HealthProcessor(float damage);
	public abstract void DecreaseMana(float amount);
	public abstract void ManaProcessor(float amount);
	public abstract void RegenProcessor();
}
