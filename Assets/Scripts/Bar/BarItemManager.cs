using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarItemManager : MonoBehaviour
{
	public Image iconImg;
	public Image cooldownImg;
	public TextMeshProUGUI slotIndexText;

	private void Start()
	{
		slotIndexText.text = (this.transform.GetSiblingIndex() + 1).ToString();
	}
}
