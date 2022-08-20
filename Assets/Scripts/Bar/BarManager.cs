using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class BarManager : MonoBehaviour
{
	[SerializeField] private SlotsData slotsData;

	[CanBeNull]
	private BarItemData m_BarItemData;

	[SerializeField] private bool isCasting;
	[SerializeField] private Transform[] buttons;
	[SerializeField] private List<BarCooldownData> barCooldownData;

	private void OnEnable()
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;
		
		InputEventManager.BarIndexPressed += BarSkillPressed;
		InputEventManager.IsCastingContinue += IsCastingContinue;
	}
	private void OnDisable()
	{
		if (this.transform.parent.GetComponent<PhotonView>().IsMine == false)
			return;
		
		InputEventManager.BarIndexPressed -= BarSkillPressed;
		InputEventManager.IsCastingContinue -= IsCastingContinue;
	}
	private void Awake()
	{
		for (int i = 0; i < slotsData.barItems.Length; i++)
		{
			if (slotsData.barItems[i] != null)
			{
				BarCooldownData newData = new BarCooldownData();

				switch (slotsData.barItems[i].barActionType)
				{
					case BarActionTypes.Empty:
						break;
					case BarActionTypes.Weapon:
						newData.barName = slotsData.barItems[i].weaponData.weaponType.ToString();
						newData.barDefaultCooldown = slotsData.barItems[i].weaponData.cooldown;
						newData.castTime = slotsData.barItems[i].weaponData.castTime;
						break;
					case BarActionTypes.Skill:
						newData.barName = slotsData.barItems[i].skillData.skillName.ToString();
						newData.barDefaultCooldown = slotsData.barItems[i].skillData.cooldown;
						newData.castTime = slotsData.barItems[i].skillData.castTime;
						break;
					case BarActionTypes.Item:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				barCooldownData.Add(newData);
			}
		}
	}
	private void Start()
	{
		for (int i = 0; i < slotsData.barItems.Length; i++)
		{
			if (slotsData.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = slotsData.barItems[i].icon;
		}
	}

	private void BarSkillPressed(int index)
	{
		m_BarItemData = slotsData.barItems[index];

		if (m_BarItemData != null)
		{
			if (barCooldownData[index].barCurrentCooldown == 0 && isCasting == false)
			{
				bool isCastSuccessful = InputEventManager.InteractionHandler(m_BarItemData);

				if (isCastSuccessful == true)
				{
					barCooldownData[index].barCurrentCooldown = barCooldownData[index].barDefaultCooldown;
					isCasting = true;
					StartCoroutine(CastingFinished(barCooldownData[index].castTime));
				}
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < barCooldownData.Count; i++)
		{
			if (barCooldownData[i].barCurrentCooldown <= 0)
			{
				barCooldownData[i].barCurrentCooldown = 0;
				barCooldownData[i].isReady = true;
			}
			else
			{
				barCooldownData[i].barCurrentCooldown -= Time.deltaTime;
				barCooldownData[i].isReady = false;
			}
		}
		for (int i = 0; i < slotsData.barItems.Length; i++) //set cooldown image
		{
			if (slotsData.barItems[i] != null)
			{
				float fillAmount = barCooldownData[i].barCurrentCooldown / barCooldownData[i].barDefaultCooldown;
				buttons[i].GetComponent<BarItemManager>().cooldownImg.DOFillAmount(fillAmount, 0.01f);
			}
			else
			{
				buttons[i].GetComponent<BarItemManager>().cooldownImg.DOFillAmount(0, 0.01f);
			}
		}
	}

	private IEnumerator CastingFinished(float value)
	{
		yield return new WaitForSeconds(value);
		isCasting = false;
	}

	private bool IsCastingContinue()
	{
		return isCasting;
	}
	[Button]
	private void OnValidate()
	{
		for (int i = 0; i < slotsData.barItems.Length; i++)
		{
			if (slotsData.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = slotsData.barItems[i].icon;
		}
	}
}
