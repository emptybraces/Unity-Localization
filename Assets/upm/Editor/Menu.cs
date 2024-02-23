using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace EmptyBraces.Localization.Editor
{
	public static class Menu
	{
		[MenuItem("Assets/Localization/Create Localization Settings")]
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

		[MenuItem("Assets/Localization/Create LID.cs", false)]
		public static void CreateLID()
		{
			cn.logf("Start");
			var path_src = Path.Combine(Application.streamingAssetsPath, Settings.Instance.LocalizeFileLocation, $"{Settings.Instance.GetDefaultLaunguageId()}_word.txt");
			var path_output_parent = Path.Combine(Application.dataPath, Settings.Instance.SourceFileLocation);
			var path_output = Path.Combine(path_output_parent, "LID.cs");
			try
			{
				var lines = File.ReadAllLines(path_src);
				var sb = new StringBuilder();
				sb.Append(@"// Auto Generated
namespace EmptyBraces.Localization
{
	public static class LID
	{
");
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
						cn.logw("detect array elements.", line);
						continue;
					}
					var key = trimmed[..idx];
					var var_name = key.Replace("/", "_");
					sb.AppendLine($"public const string {var_name} = \"{key}\";");
				}
				sb.AppendLine("}");
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
			cn.logf("Compoleted");
		}
		[MenuItem("Assets/Localization/Create LID.cs", true)]
		public static bool CreateLIDValidate()
		{
			return null != Resources.Load<Settings>(LocalizationManager.k_SettingsFileName);
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