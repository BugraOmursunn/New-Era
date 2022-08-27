using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "BarItems/SpellData", order = 2)]
public class SpellData : ScriptableObject
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
	public SpellType Type;

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
	public class SpellVFXSettings
	{
		public bool isChild;
		public GameObject Prefab;
		public AnimationClip Animation;
		public string AnimTriggerName;
		public float vfxActivationTime;
		public Vector3 offSet;
	}
	[VerticalGroup("Split/Right")]
	[BoxGroup("Split/Right/Animation Settings")]
	[HideLabel]
	public SpellVFXSettings spellVFXSettings;


	[Serializable]
	public class SpellModifierSettings
	{
		[SuffixLabel("seconds ", true)]
		public float cooldown;
		[SuffixLabel("seconds ", true)]
		public float castTime;
	}
	[BoxGroup("Split/Right/Modifier Settings")]
	[HideLabel]
	public SpellModifierSettings spellModifierSettings;

	//public StatList modifiers;

	// [TabGroup("Starting Inventory")]
	// public string Notes2;

	// [BoxGroup(STATS_BOX_GROUP)]
	// public int ItemStackSize = 1;
	//
	// [BoxGroup(STATS_BOX_GROUP)]
	// public float ItemRarity;

	// [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
	// [BoxGroup("Split/Left/Notes")]
	// [HideLabel, TextArea(6, 14)]
	// public string Notes;
	//
	// public bool Toggle = true;
	//
	// [ShowIfGroup("Toggle")]
	// [BoxGroup("Toggle/Shown Box")]
	// public int A, B;
	//
	// [BoxGroup("Box")]
	// public InfoMessageType EnumField = InfoMessageType.Info;
	//
	// [BoxGroup("Box")]
	// [ShowIfGroup("Box/Toggle")]
	// public Vector3 X, Y;
	//
	// // Like the regular If-attributes, ShowIfGroup also supports specifying values.
	// // You can also chain multiple ShowIfGroup attributes together for more complex behaviour.
	// [ShowIfGroup("Box/Toggle/barActionType", Value = BarActionTypes.Spell)]
	// [BoxGroup("Box/Toggle/barActionType/Border", ShowLabel = false)]
	// public string Name2;
	// public WeaponData weaponData2;
}
