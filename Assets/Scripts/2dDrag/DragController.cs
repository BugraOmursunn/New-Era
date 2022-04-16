using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	private RectTransform _rectTransform;
	private void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
	}
	public void OnDrag(PointerEventData eventData)
	{
		_rectTransform.anchoredPosition += eventData.delta;

		Vector2 clampVector2 = new Vector2();
		clampVector2 = _rectTransform.anchoredPosition;

		float clampX = ((float)Screen.width / 2) - _rectTransform.sizeDelta.x / 2;
		float clampY = ((float)Screen.height / 2) - _rectTransform.sizeDelta.y / 2;

		clampVector2.x = Mathf.Clamp(clampVector2.x, -clampX, clampX);
		clampVector2.y = Mathf.Clamp(clampVector2.y, -clampY, clampY);
		_rectTransform.anchoredPosition = clampVector2;
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		_rectTransform.anchoredPosition = Vector2.zero;
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Test");
	}





	// #region RaycastExample
	// Debug.DrawRay(_rectTransform.position, Vector3.right * 1000, UnityEngine.Color.red);
	// RaycastHit2D hit = Physics2D.Raycast(_rectTransform.position, Vector2.right, 1000);
	//
	// if (hit.collider != null)
	// {
	// 	Debug.Log("Test");
	// }
	// #endregion
}
