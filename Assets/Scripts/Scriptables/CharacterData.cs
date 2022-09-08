using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 6)]
public class CharacterData : ScriptableObject
{
	protected const string LEFT_VERTICAL_GROUP = "Split/Left";
	protected const string RIGHT_VERTICAL_GROUP = "Split/Right";
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
	public ClassTypes Class;

	[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
	public GameObject DamageIndicator;

	[VerticalGroup(LEFT_VERTICAL_GROUP)]
	[BoxGroup("Split/Left/Description")]
	[HideLabel, TextArea(6, 14)]
	public string Description;
	[BoxGroup("Split/Left/Notes")]
	[HideLabel, TextArea(6, 14)]
	public string Notes;

	[VerticalGroup("Split/Right")]
	[BoxGroup("Split/Right/Class Settings")]
	public SpellBook SpellBook;

	[BoxGroup("Split/Right/Class Settings")]
	public WeaponData Weapon;

	[Serializable]
	public class CharacterStats
	{
		[ProgressBar(0, 1000), ShowInInspector]
		public float Health;
		[ProgressBar(0, 1000), ShowInInspector]
		public float Mana;
		[ProgressBar(0, 1000), ShowInInspector]
		public float Stamina;

		[SuffixLabel("perSec ", true)]
		public float healthRegen;
		[SuffixLabel("perSec ", true)]
		public float manaRegen;
		[SuffixLabel("perSec ", true)]
		public float staminaRegen;
		
	}
	[BoxGroup("Split/Right/Stat Settings")]
	[HideLabel]
	public CharacterStats characterStats;

	[BoxGroup("Split/Right/Animation Settings")]
	public string GetHitAnim;
	[BoxGroup("Split/Right/Animation Settings")]
	public string DieAnim;
}
