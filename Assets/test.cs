using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	
	// Start is called before the first frame update
	void Start()
	{
		TestClass myTest = new TestClass();
		Debug.Log(myTest.Lives);
	}

	// Update is called once per frame
	void Update()
	{
	}
}
