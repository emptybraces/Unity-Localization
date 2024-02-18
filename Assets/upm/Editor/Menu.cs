using System.IO;
using UnityEngine;
using UnityEditor;
namespace EmptyBraces.Localization.Editor
{
	public static class Menu
	{
		[MenuItem("Assets/Create Localization Settings")]
		public static void MakeSettingsAssetIfNeeded()
		{
			if (Resources.Load<Settings>(LocalizationManager.k_SettingsFileName) == null)
			{
				var path = Application.dataPath + "/Resources";
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(Settings)), $"Assets/Resources/{LocalizationManager.k_SettingsFileName}.asset");
				AssetDatabase.Refresh();
			}
		}
	}
}