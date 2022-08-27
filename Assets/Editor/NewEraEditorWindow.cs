using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class NewEraEditorWindow : OdinMenuEditorWindow
{
	[MenuItem("New Era/Game Editor")]
	private static void OpenWindow()
	{
		var window = GetWindow<NewEraEditorWindow>();
		window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
	}

	protected override OdinMenuTree BuildMenuTree()
	{
		// var tree = new OdinMenuTree();
		// tree.Selection.SupportsMultiSelect = false;
		//
		// tree.Add("Spells", new CreateNewEnemyData());
		// tree.AddAllAssetsAtPath("Spells", "Assets/MyResources/2hActions", typeof(BarItemData));
		// return tree;

		var tree = new OdinMenuTree(true);
		tree.DefaultMenuStyle.IconSize = 28.00f;
		tree.Config.DrawSearchToolbar = true;

		// Adds the character overview table.
		//CharacterOverview.Instance.UpdateCharacterOverview();
		//tree.Add("Spells", new CreateNewSpellData());

		// Adds all characters.

		tree.AddAllAssetsAtPath("Spells", "Assets/MyResources/Spells", typeof(SpellData), true, true);
		tree.AddAllAssetsAtPath("Weapons", "Assets/MyResources/Weapons", typeof(WeaponData), true, true);

		// Add all scriptable object items.
		//tree.AddAllAssetsAtPath("", "Assets/Plugins/Sirenix/Demos/SAMPLE - RPG Editor/Items", typeof(Item), true)
		//.ForEach(this.AddDragHandles);

		// Add drag handles to items, so they can be easily dragged into the inventory if characters etc...
		//tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);

		// Add icons to characters and items.
		tree.EnumerateTree().AddIcons<SpellData>(x => x.textureIon);
		tree.EnumerateTree().AddIcons<WeaponData>(x => x.textureIon);
		return tree;
	}

	// public class CreateNewSpellData
	// {
	// 	public CreateNewSpellData()
	// 	{
	// 		spellData = ScriptableObject.CreateInstance<BarItemData>();
	// 		spellData.Name = "New Enemy Data";
	// 	}
	//
	// 	[InlineEditor(Expanded = true)]
	// 	public BarItemData spellData;
	//
	// 	[Button("Add New Enemy SO")]
	// 	private void CreateNewData()
	// 	{
	// 		AssetDatabase.CreateAsset(spellData, "Assets/MyResources/2hActions/" + spellData.Name + ".asset");
	// 		AssetDatabase.SaveAssets();
	// 	}
	// }
	//
	protected override void OnBeginDrawEditors()
	{
		var selected = this.MenuTree.Selection.FirstOrDefault();

		SirenixEditorGUI.BeginHorizontalToolbar();
		{
			if (selected != null)
			{
				GUILayout.Label(selected.Name);
			}

			GUILayout.FlexibleSpace();

			// if (SirenixEditorGUI.ToolbarButton("Delete Current"))
			// {
			// 	BarItemData asset = selected.SelectedValue as BarItemData;
			// 	string path = AssetDatabase.GetAssetPath(asset);
			// 	AssetDatabase.DeleteAsset(path);
			// 	AssetDatabase.SaveAssets();
			// }
		}
		SirenixEditorGUI.EndHorizontalToolbar();
	}
}
