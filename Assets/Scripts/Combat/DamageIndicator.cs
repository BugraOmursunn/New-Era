using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
	private PhotonView view;

	public void Instantiate(float _damage)
	{
		GameTypes gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.SinglePlayer)
		{
			RPCInstantiate(_damage);
		}
		else if (gameType == GameTypes.MultiPlayer)
		{
			view = this.GetComponent<PhotonView>();
			view.RPC(nameof(RPCInstantiate), RpcTarget.All, _damage);
		}
	}

	[PunRPC]
	public void RPCInstantiate(float _damage)
	{
		if (view == null)
			return;
		if (view.IsMine == false)
			return;

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
