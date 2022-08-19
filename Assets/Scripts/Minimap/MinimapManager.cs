using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
	public Transform target;
	[SerializeField] private Vector3 offset;

	[SerializeField] private bool rotateWithPlayer;
	[SerializeField] private Camera minimapCam;

	[SerializeField] private Transform miniMapMask;
	[SerializeField] private Transform miniMap;
	[SerializeField] private float mapSizeValue;
	[SerializeField] private float mapZoomValue = 5;

	[SerializeField] private float iconSizeValue = 1f;
	[SerializeField] private List<GameObject> miniMapIcons;

	private void Start()
	{
		SetValues();
	}
	private void OnValidate()
	{
		SetValues();
	}

	private void SetValues()
	{
		miniMapIcons = GameObject.FindGameObjectsWithTag("MinimapIcon").ToList();
		if (miniMapIcons != null)
		{
			foreach (GameObject item in miniMapIcons)
			{
				item.transform.localScale = new Vector3(iconSizeValue, iconSizeValue, iconSizeValue);
			}
		}

		minimapCam.orthographicSize = mapZoomValue;
		miniMapMask.localScale = Vector3.one * mapSizeValue;
	}

	private void Update()
	{
		minimapCam.transform.position = target.position + offset;
		if (rotateWithPlayer)
		{
			//miniMap.transform.eulerAngles = new Vector3(0f, 0f, target.transform.eulerAngles.y);
			miniMap.transform.rotation = Quaternion.Euler(0f, 0f, target.transform.eulerAngles.y);
		}
	}
}
