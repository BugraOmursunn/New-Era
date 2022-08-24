using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClass
{
	public int Lives { get; private set; }

	
	public TestClass(int lives = 0)
	{
		Lives = lives;
	}
}
