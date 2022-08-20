using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	[SerializeField] private Animator playerAnimator;

	private WeaponData weaponData;

	private bool isDraw;

	[Space(10)]
	[SerializeField] private List<HandData> handData;

	[Space(10)]
	[SerializeField] private bool isHaveSword2h;
	[SerializeField] private Transform sword2hSpawnPos;
	[SerializeField] private GameObject sword2hPrefab;
	[SerializeField] private GameObject sword2h;


	private Transform sword2hParent;
	private Transform sword2hObject;

	private WeaponAttachmentData weaponAttachmentData;
	private FullBodyBipedIK bidepIK;

	private void OnEnable()
	{
		InputEventManager.DrawWeapon += DrawWeapon;
		InputEventManager.Attack += OnAttack;
		InputEventManager.IsDrawSword2h += IsDrawSword2h;
	}
	private void OnDisable()
	{
		InputEventManager.DrawWeapon -= DrawWeapon;
		InputEventManager.Attack -= OnAttack;
		InputEventManager.IsDrawSword2h -= IsDrawSword2h;
	}
	private void Awake()
	{
		if (isHaveSword2h)
		{
			sword2h = Instantiate(sword2hPrefab, sword2hSpawnPos);
			weaponAttachmentData = sword2h.GetComponent<WeaponAttachmentData>();
			sword2hParent = sword2h.transform;
			sword2hObject = sword2h.transform.GetChild(0);
		}
		bidepIK = this.GetComponent<FullBodyBipedIK>();
	}
	private void DrawWeapon(WeaponData _weaponData)
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;

		weaponData = _weaponData;

		isDraw = !isDraw;
		switch (weaponData.weaponType)
		{
			case WeaponType.Empty:
				break;
			case WeaponType.Sword_1h_shield:
				break;
			case WeaponType.Sword_2h:
				playerAnimator.SetTrigger(isDraw ? "Draw2hSword" : "Sheath2hSword");
				playerAnimator.SetBool("isEmptyHand", !isDraw);
				playerAnimator.SetBool("is2hDraw", isDraw);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private bool IsDrawSword2h()
	{
		return isDraw;
	}
	private void OnAttack()
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;

		if (isDraw == true && InputEventManager.IsCastingContinue() == false)
		{
			playerAnimator.SetTrigger("Attack");
		}
	}

	public void AttachWeaponToHand()
	{
		sword2hObject.parent = handData[1].hand;
		sword2hObject.localPosition = weaponAttachmentData.handAttachPos;
		sword2hObject.localRotation = Quaternion.Euler(weaponAttachmentData.handAttachRot);
	}

	public void AttackWeaponToSheath()
	{
		sword2hObject.parent = sword2hParent.transform;
		sword2hObject.transform.localPosition = weaponAttachmentData.sheathAttachPos;
		sword2hObject.localRotation = Quaternion.Euler(weaponAttachmentData.sheathAttachRot);
	}
	public void EnableWeaponTrail()
	{
		InputEventManager.EnableWeaponTrail.Invoke();
	}
	public void DisableWeaponTrail()
	{
		InputEventManager.DisableWeaponTrail.Invoke();
	}

	public void AttachRightHandToWeapon()
	{
		//bidepIK.solver.rightHandEffector.target = weaponAttachmentData.rightHandAttachTransform;
	}
	public void AttachLeftHandToWeapon()
	{
		SwitchHandAttackStatus(true);
	}
	public void DetachLeftHandFromWeapon()
	{
		SwitchHandAttackStatus(false);
	}
	private Sequence seq;

	private void SwitchHandAttackStatus(bool targetStatus)
	{
		if (targetStatus == true && bidepIK.solver.leftHandEffector.positionWeight == 0)
		{
			if (isDraw == false)
				return;

			var positionWeightTarget = bidepIK.solver.leftHandEffector.positionWeight;
			var rotationWeightTarget = bidepIK.solver.leftHandEffector.rotationWeight;
			var maintainRelativePositionWeight = bidepIK.solver.leftHandEffector.maintainRelativePositionWeight;

			bidepIK.solver.leftHandEffector.target = weaponAttachmentData.handData[0].hand;

			seq.Kill();
			if (bidepIK.solver.leftHandEffector.target == null)
				SwitchHandAttackStatus(false);

			seq.Append(DOTween.To(() => positionWeightTarget, x => positionWeightTarget = x, weaponAttachmentData.handData[0].positionWeight, 0.3f)
				.OnUpdate(() => {
					bidepIK.solver.leftHandEffector.positionWeight = positionWeightTarget;
				}).SetEase(Ease.Linear));
			seq.Join(DOTween.To(() => rotationWeightTarget, x => rotationWeightTarget = x, weaponAttachmentData.handData[0].rotationWeight, 0.3f)
				.OnUpdate(() => {
					bidepIK.solver.leftHandEffector.rotationWeight = rotationWeightTarget;
				}).SetEase(Ease.Linear));
			seq.Join(DOTween.To(() => maintainRelativePositionWeight, x => maintainRelativePositionWeight = x, weaponAttachmentData.handData[0].maintainRelativePositionWeight, 0.3f)
				.OnUpdate(() => {
					bidepIK.solver.leftHandEffector.maintainRelativePositionWeight = maintainRelativePositionWeight;
				}).SetEase(Ease.Linear));
		}
		else if (targetStatus == false && Math.Abs(bidepIK.solver.leftHandEffector.positionWeight - 1) < 0.2f)
		{
			bidepIK.solver.leftHandEffector.target = null;
		}
	}
	private void LateUpdate()
	{
		if (bidepIK.solver.leftHandEffector.target == null)
		{
			seq.Kill();
			bidepIK.solver.leftHandEffector.positionWeight = 0;
			bidepIK.solver.leftHandEffector.rotationWeight = 0;
			bidepIK.solver.leftHandEffector.maintainRelativePositionWeight = 0;
		}
	}
}
