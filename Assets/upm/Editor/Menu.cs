using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
namespace EmptyBraces.Localization.Editor
{
	public static class Menu
	{
		const string k_MenuPath = "Assets/Localization";
		[MenuItem(k_MenuPath + "/Create Localization Settings")]
		public static void MakeSettingsAssetIfNeeded()
		{
			var asset = Resources.Load<ScriptableObject>(LocalizationManager.k_SettingsFileName);
			if (asset == null)
			{
				var path = Application.dataPath + "/Resources";
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				asset = ScriptableObject.CreateInstance(typeof(Settings));
				AssetDatabase.CreateAsset(asset, $"Assets/Resources/{LocalizationManager.k_SettingsFileName}.asset");
				AssetDatabase.Refresh();
			}
			EditorGUIUtility.PingObject(asset);
		}

		[MenuItem(k_MenuPath + "/Create LID.cs", false)]
		public static void CreateLID()
		{
			Debug.Log("CreateLID: Start");
			var path_src = Path.Combine(Application.streamingAssetsPath, Settings.Instance.LocalizeFileLocation, $"{Settings.Instance.GetDefaultLaunguageId()}_word.txt");
			var path_output_parent = Path.Combine(Application.dataPath, Settings.Instance.AutoGenerateLocalizeKeyFileLocation);
			var path_output = Path.Combine(path_output_parent, "LID.cs");
			try
			{
				var lines = File.ReadAllLines(path_src);
				var sb = new StringBuilder();
				sb.AppendLine("// Auto Generated");
				if (!string.IsNullOrEmpty(Settings.Instance.AutoGenerateLocalizeKeyFileNamespace))
					sb.AppendLine($"namespace {Settings.Instance.AutoGenerateLocalizeKeyFileNamespace}\n{{");
				sb.AppendLine("\tpublic static class LID\n\t{");

				foreach (var line in lines)
				{
					// 3文字以下はあり得ない
					var trimmed = line.Trim();
					if (trimmed.Length < 3)
						continue;
					if (trimmed.StartsWith("/", StringComparison.Ordinal))
						continue;
					var idx = trimmed.IndexOfAny(new char[] { '\t', ' ' });
					if (idx == -1)
					{
						if (Settings.Instance.EnableDebugLog)
							Debug.Log($"Detect array elements. {line}");
						continue;
					}
					var key = trimmed[..idx];
					var var_name = key.Replace("/", "_");
					sb.AppendLine($"\t\tpublic const string {var_name} = \"{key}\";");
				}
				sb.AppendLine("\t}");
				if (!string.IsNullOrEmpty(Settings.Instance.AutoGenerateLocalizeKeyFileNamespace))
					sb.AppendLine("}");
				if (!Directory.Exists(path_output_parent))
					Directory.CreateDirectory(path_output_parent);
				File.WriteAllText(path_output, sb.ToString(), Encoding.UTF8);
				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return;
			}
			Debug.Log("CreateLID: Completed.");
		}
		[MenuItem(k_MenuPath + "/Create LID.cs", true)]
		public static bool CreateLIDValidate()
		{
			return null != Resources.Load<Settings>(LocalizationManager.k_SettingsFileName);
		}

		[MenuItem(k_MenuPath + "/Dump TMP_Text without base font registered in Settings.")]
		static void _DumpTMPTextWithoutBaseFont()
		{
			Debug.Log("Start");
			var base_fonts = Settings.Instance.SupportLanguageFontAssets.Select(e => e.BaseFontAsset).ToArray();
			var is_detect = false;
			foreach (var tmp in GameObject.FindObjectsByType<TMPro.TMP_Text>(FindObjectsSortMode.None))
			{
				if (!base_fonts.Contains(tmp.font))
				{
					if (!is_detect && (is_detect = true))
						Debug.LogWarning("Detects!");
					Debug.LogWarning(SearchUtils.GetHierarchyPath(tmp.gameObject, true), tmp.gameObject);
				}
			}
			if (!is_detect)
				Debug.Log("Nothing Detects.");
			Debug.Log("Finish");
		}

		public class Importer : AssetPostprocessor
		{
			static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
				foreach (var i in importedAssets)
				{
					if (Settings.Instance != null)
					{
						foreach (var j in Settings.Instance.SupportLanguages)
						{
							if (i.EndsWith($"{j.Prefix}_word.txt", StringComparison.Ordinal))
							{
								CreateLID();
								break;
							}
						}
					}
					else
					{
						break;
					}
				}
			}
		}
	}
}