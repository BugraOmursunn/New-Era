using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
	[SerializeField] private Animator animator;
	public void ChangeAnimatorState(int value)
	{
		animator.SetInteger("State", value);
	}
	public void SinglePlayer()
	{
		SceneManager.LoadScene("BattleArea");
	}
	public void ExitGame()
	{
		Application.Quit();
	}
}
