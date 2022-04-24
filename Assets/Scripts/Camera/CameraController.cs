using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera virtualCamera;
	private CinemachineComponentBase componentBase;
	private float zoomValue;
	private void OnEnable()
	{
		InputEventManager.ChangeCameraZoomValue += ChangeCameraZoomValue;
	}
	private void OnDisable()
	{
		InputEventManager.ChangeCameraZoomValue -= ChangeCameraZoomValue;
	}
	private void ChangeCameraZoomValue(float value)
	{
		if (componentBase == null)
		{
			componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
		}

		if (value > 0)
		{
			zoomValue = (componentBase as Cinemachine3rdPersonFollow).CameraDistance;
			DOTween.To(x => (componentBase as Cinemachine3rdPersonFollow).CameraDistance = x, zoomValue, zoomValue - 1, 0.5f);
		}
		else
		{
			zoomValue = (componentBase as Cinemachine3rdPersonFollow).CameraDistance;
			DOTween.To(x => (componentBase as Cinemachine3rdPersonFollow).CameraDistance = x, zoomValue, zoomValue + 1, 0.5f);
		}
	}
}
