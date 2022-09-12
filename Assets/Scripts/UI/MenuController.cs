using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	[SerializeField] private Animator animator;
	public void ChangeAnimatorState(int value)
	{
		animator.SetInteger("State", value);
	}
}
