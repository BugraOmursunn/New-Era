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

	[SerializeField] private WeaponData weaponData;

	private bool isDraw;

	[Space(10)]
	[SerializeField] private Transform skeletonChestPos;
	[SerializeField] private List<HandData> handData;

	[Space(10)]
	private GameObject weaponGameObject;
	private Transform weaponParent;
	private Transform weaponTransform;

	private WeaponAttachmentData weaponAttachmentData;
	private FullBodyBipedIK bidepIK;

	private GameTypes gameType;
	private void OnEnable()
	{
		gameType = EventManager.gameType.Invoke();
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		InputEventManager.DrawWeapon += DrawWeapon;
		InputEventManager.Attack += OnAttack;
		InputEventManager.IsDrawSword2h += IsDrawSword2h;
	}
	private void OnDisable()
	{
		if (gameType == GameTypes.MultiPlayer)
		{
			if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
				return;
		}

		InputEventManager.DrawWeapon -= DrawWeapon;
		InputEventManager.Attack -= OnAttack;
		InputEventManager.IsDrawSword2h -= IsDrawSword2h;
	}
	private void Awake()
	{
		weaponData = EventManager.GetCharacterData.Invoke().Weapon;
		weaponGameObject = Instantiate(weaponData.weaponVFXSettings.Prefab, skeletonChestPos);
		weaponParent = weaponGameObject.transform;
		weaponTransform = weaponGameObject.transform.GetChild(0);
		weaponAttachmentData = weaponGameObject.GetComponent<WeaponAttachmentData>();

		bidepIK = this.GetComponent<FullBodyBipedIK>();
	}
	private void DrawWeapon(WeaponData _weaponData)
	{
		weaponData = _weaponData;

		isDraw = !isDraw;

		playerAnimator.SetTrigger(isDraw ? weaponData.weaponVFXSettings.drawAnimName : weaponData.weaponVFXSettings.sheetAnimName);
		playerAnimator.SetBool("isEmptyHand", !isDraw);
		if (weaponData.weaponVFXSettings.is2Hand == true)
			playerAnimator.SetBool("is2hDraw", isDraw);
	}

	private bool IsDrawSword2h()
	{
		return isDraw;
	}
	private void OnAttack()
	{
		if (isDraw == true && InputEventManager.IsCastingContinue() == false)
			playerAnimator.SetTrigger("Attack");
	}

	public void AttachWeaponToHand()
	{
		weaponTransform.parent = handData[1].hand;
		weaponTransform.localPosition = weaponAttachmentData.handAttachPos;
		weaponTransform.localRotation = Quaternion.Euler(weaponAttachmentData.handAttachRot);
	}

	public void AttackWeaponToSheath()
	{
		weaponTransform.parent = weaponParent.transform;
		weaponTransform.transform.localPosition = weaponAttachmentData.sheathAttachPos;
		weaponTransform.localRotation = Quaternion.Euler(weaponAttachmentData.sheathAttachRot);
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
		SwitchHandIKStatus(true);
	}
	public void DetachLeftHandFromWeapon()
	{
		SwitchHandIKStatus(false);
	}
	private Sequence seq;

	private void SwitchHandIKStatus(bool targetStatus)
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
				SwitchHandIKStatus(false);

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
