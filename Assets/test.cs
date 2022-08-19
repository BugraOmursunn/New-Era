using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class test : MonoBehaviour
{
	[Button]
	private void TestVoid()
	{
		float testA = 0f;
		Sequence seq = DOTween.Sequence();
		seq.Kill();

		seq.Append(DOTween.To(() => testA, x => testA = x, 2, 0.3f)
			.OnUpdate(() => {
				Debug.Log(testA);
			}).SetEase(Ease.Linear));
	}
}
