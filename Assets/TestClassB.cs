using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClassB : MonoBehaviour,IInteraction
{
    public void TriggerInteraction()
    {
        Debug.Log("ClassB");
    }
}
