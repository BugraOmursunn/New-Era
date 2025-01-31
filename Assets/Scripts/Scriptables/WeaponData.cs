using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "BarItems/WeaponData", order = 3)]
public class WeaponData : ScriptableObject
{
	protected const string LEFT_VERTICAL_GROUP = "Split/Left";
	protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

	[HorizontalGroup("Split", 0.5f, MarginRight = 10, LabelWidth = 130)]
	[HideLabel, PreviewField(55)]
	[VerticalGroup(LEFT_VERTICAL_GROUP)]
	[HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
	public Texture textureIon;

	[BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
	[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
	public string Name;

	[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
	public WeaponType Type;

	[AssetsOnly]
	[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
	public Sprite Icon;

	[BoxGroup("Split/Left/Description")]
	[HideLabel, TextArea(6, 14)]
	public string Description;

	[BoxGroup("Split/Left/Notes")]
	[HideLabel, TextArea(6, 14)]
	public string Notes;

	
	[Serializable]
	public class WeaponModifierSettings
	{
		public float attackDamage;
		[SuffixLabel("seconds ", true)]
		public float cooldown;
		[SuffixLabel("seconds ", true)]
		public float castTime;
	}
	[BoxGroup("Split/Right/Modifier Settings")]
	[HideLabel]
	public WeaponModifierSettings weaponModifierSettings;
	
	
	[Serializable]
	public class WeaponVFXSettings
	{
		public bool is2Hand;
		public GameObject Prefab;
		public string drawAnimName;
		public string sheetAnimName;
	}
	[VerticalGroup("Split/Right")]
	[BoxGroup("Split/Right/Animation Settings")]
	[HideLabel]
	public WeaponVFXSettings weaponVFXSettings;


	// [VerticalGroup("Split/Right")]
	// public StatList modifiers;
}
