using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DummyTarget : MonoBehaviour, IDamageAble
{
	public Animator animator;
	public GameObject damageIndicator;
	public void GetDamage(float damage)
	{
		animator.SetTrigger("GetHit");

		GameObject newIndicator = Instantiate(damageIndicator);
		TextMeshPro damageText = newIndicator.transform.GetChild(0).GetComponent<TextMeshPro>();
		damageText.color = damage > 0 ? Color.green : Color.red;

		Sequence seq = DOTween.Sequence();
		seq.AppendCallback(() => {
			newIndicator.transform.position = this.transform.position;
			damageText.text = damage.ToString(CultureInfo.InvariantCulture);
		});
		seq.Append(newIndicator.transform.DOLocalMoveY(3, 0.8f).From(2).SetEase(Ease.Linear)).OnUpdate(() => {
			if (newIndicator != null)
				newIndicator.transform.DOLookAt(Camera.main.transform.position, 0.01f);
		});
		seq.AppendCallback(() => {
			DestroyImmediate(newIndicator);
		});
	}
}
