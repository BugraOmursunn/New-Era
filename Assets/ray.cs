using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ray : MonoBehaviour
{
	[SerializeField] private LayerMask _layerMask;
	// Update is called once per frame
	void Update()
	{
		Debug.DrawRay(this.transform.position, Vector3.forward * 50, Color.red, 0.01f);
		RaycastHit hit;
		Ray ray = new Ray(this.transform.position, this.transform.forward * 50);

		if (Physics.Raycast(ray, out hit,_layerMask))
		{
			Debug.Log(hit.transform.name);
			//transform.position = hit.point;

			// Do something with the object that was hit by the raycast.
		}
	}
}
