using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;

namespace Emptybraces.Localization.Editor
{
	public static class Extensions
	{
		// https://forum.unity.com/threads/set-addressable-via-c.671938/#post-6819911
		public static AddressableAssetEntry SetAddressableGroup(this Object obj, string groupName)
		{
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			if (settings)
			{
				var group = settings.FindGroup(groupName);
				if (!group)
					group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

				var assetpath = AssetDatabase.GetAssetPath(obj);
				var guid = AssetDatabase.AssetPathToGUID(assetpath);

				var e = settings.CreateOrMoveEntry(guid, group, false, false);
				var entriesAdded = new List<AddressableAssetEntry> { e };

				group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
				settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
				return e;
			}
			return null;
		}

		public static T[] FindObjectsByType<T>(FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude, FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None) where T : Component
		{
			if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage)
				return stage.FindComponentsOfType<T>();
			else
				return GameObject.FindObjectsByType<T>(findObjectsInactive, findObjectsSortMode);
		}
	}
}
