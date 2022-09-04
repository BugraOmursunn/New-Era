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
		if ((view == null || view.IsMine == false) && EventManager.gameType.Invoke() == GameTypes.MultiPlayer)
		{
			Destroy(this.gameObject);
			return;
		}

		StartCoroutine(Delay());

		TextMeshPro damageText = this.transform.GetChild(0).GetComponent<TextMeshPro>();
		damageText.color = _damage > 0 ? Color.green : Color.red;

		Sequence seq = DOTween.Sequence();
		seq.AppendCallback(() => {
			damageText.text = _damage.ToString(CultureInfo.InvariantCulture);
		});
		seq.Append(this.transform.DOMoveY(this.transform.position.y + 2, 1f)).SetEase(Ease.Linear).OnUpdate(() => {
			if (this != null)
				this.transform.DOLookAt(Camera.main.transform.position, 0.01f);
		});
	}
	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(1);

		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.Destroy(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}
}
