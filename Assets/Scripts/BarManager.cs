using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
	[SerializeField] private BarControllerData barControllerData;
	[SerializeField] private  Transform[] buttons;
	private void Start()
	{
		for (int i = 0; i < barControllerData.slots.barItems.Length; i++)
		{
			if (barControllerData.slots.barItems[i] != null)
				buttons[i].GetComponent<BarItemManager>().iconImg.sprite = barControllerData.slots.barItems[i].icon;
			
		}
	}
}
