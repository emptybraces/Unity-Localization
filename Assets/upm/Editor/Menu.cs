using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
namespace Emptybraces.Localization.Editor
{
	public static class Menu
	{
		const string k_MenuPath = "Assets/Localization";
		[MenuItem(k_MenuPath + "/Create Localization Settings")]
		public static void MakeSettingsAssetIfNeeded()
		{
			if (!Settings.Editor_Load(out var asset))
			{
				asset = ScriptableObject.CreateInstance<Settings>();
				AssetDatabase.CreateAsset(asset, $"Assets/{LocalizationManager.k_SettingsFileName}.asset");
			}
			var entry = asset.SetAddressableGroup(LocalizationManager.k_AddressablesGroupName);
			entry.SetAddress(asset.name);
			AssetDatabase.Refresh();
			EditorGUIUtility.PingObject(asset);
		}

		[MenuItem(k_MenuPath + "/Create LID.cs", false)]
		public static void CreateLID()
		{
			Debug.Log("[LocalizationManager] CreateLID: Start");
			if (Settings.Instance.GetDefaultLaunguageId() is null)
			{
				Debug.LogError("[LocalizationManager] Please add a DefaultLanguage configuration item to SupportLanguages.");
				return;
			}
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
					// コメント
					if (line.StartsWith("/", StringComparison.Ordinal))
						continue;
					// 配列
					if (line.StartsWith(" ", StringComparison.Ordinal) || line.StartsWith("\t", StringComparison.Ordinal) || line.StartsWith("　", StringComparison.Ordinal))
					{
						if (Settings.Instance.EnableDebugLog)
							Debug.Log($"[LocalizationManager] Detect array elements. {line}");
						continue;
					}
					// キーとセパレータとバリュー合わせて、3文字以下はあり得ない
					var trimmed = line.Trim();
					if (trimmed.Length < 3)
						continue;
					var idx = trimmed.IndexOfAny(new char[] { '\t', ' ', '　' });
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
			Debug.Log("[LocalizationManager] CreateLID: Completed.");
		}
		[MenuItem(k_MenuPath + "/Create LID.cs", true)]
		public static bool CreateLIDValidate()
		{
			return Settings.Editor_Load(out _);
		}

		[MenuItem(k_MenuPath + "/Dump TMP_Text without base font registered in Settings.")]
		public static void _DumpTMPTextWithoutBaseFont()
		{
			Debug.Log("[LocalizationManager] Start");
			var base_fonts = Settings.Instance.SupportLanguageFontAssets.Select(e => e.BaseFontAsset).ToArray();
			var is_detect = false;
			foreach (var tmp in Extensions.FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include))
			{
				if (!base_fonts.Contains(tmp.font))
				{
					if (!is_detect && (is_detect = true))
						Debug.LogWarning("[LocalizationManager] Detects!");
					Debug.LogWarning("[LocalizationManager] " + SearchUtils.GetHierarchyPath(tmp.gameObject, true), tmp.gameObject);
				}
			}
			if (!is_detect)
				Debug.Log("[LocalizationManager] Nothing Detects.");
			Debug.Log("[LocalizationManager] Finish");
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
