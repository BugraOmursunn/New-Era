using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class SkillProcessor
{
	private static Dictionary<SkillType, Skill> _skills = new Dictionary<SkillType, Skill>();
	private static bool _initialized;

	private static void Initialize()
	{
		_skills.Clear();

		var assembly = Assembly.GetAssembly(typeof(Skill));

		var allSkillTypes = assembly.GetTypes()
			.Where(t => typeof(Skill).IsAssignableFrom(t) && t.IsAbstract == false);

		foreach (var skillType in allSkillTypes)
		{
			Skill skill = Activator.CreateInstance(skillType) as Skill;
			_skills.Add(skill.SkillType, skill);
		}
	}

	public static void CastSkill(SkillType skillType, Vector3 center, float radius, int damage)
	{
		if (_initialized == false)
			Initialize();

		var skill = _skills[skillType];
		skill.CastSkill(center, radius, damage);
	}
}
