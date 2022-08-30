using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellBook", menuName = "ScriptableObjects/SpellBook", order = 2)]
public class SpellBook : ScriptableObject
{
	public Texture textureIon;
	public List<ScriptableObject> barItems;
}
