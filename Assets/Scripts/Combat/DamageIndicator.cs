using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
	public void Instantiate(float _damage)
	{
		Destroy(this.gameObject, 1f);
		
		TextMeshPro damageText = this.transform.GetChild(0).GetComponent<TextMeshPro>();
		damageText.color = _damage > 0 ? Color.green : Color.red;

		Sequence seq = DOTween.Sequence();
		seq.AppendCallback(() => {
			damageText.text = _damage.ToString(CultureInfo.InvariantCulture);
		});
		seq.Append(this.transform.DOLocalMoveY(3, 0.8f).From(2).SetEase(Ease.Linear)).OnUpdate(() => {
			if (this != null)
				this.transform.DOLookAt(Camera.main.transform.position, 0.01f);
		});
	}
}
