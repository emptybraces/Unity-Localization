using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace Nnfs.Editor
{
	public static class LocalizationEditor
	{
		[MenuItem("Nnfs/Make LID", false)]
		public static void MakeLID()
		{
			var path_src = Application.dataPath + "/StreamingAssets/localization/ja_word.txt";
			var path_output = Application.dataPath + "/_script/LID.cs";
			try
			{
				var lines = File.ReadAllLines(path_src);
				var sb = new StringBuilder();
				sb.Append(@"// Auto Generated
namespace Nnfs
{
	public static class LID
	{
");
				foreach (var line in lines)
				{
					if (line.Length < 3)
						continue;
					if (line.StartsWith("/"))
						continue;
					var idx = line.IndexOfAny(new char[] { '\t', ' ' });
					if (idx == -1)
					{
						cn.loge("キーだけで内容が定義されていません。", line);
						return;
					}
					var key = line[..idx].Trim();
					if (key == "")
						continue;
					var var_name = key.Replace("/", "_");
					sb.AppendLine($"public static readonly string {var_name} = \"{key}\";");
				}
				sb.AppendLine("}");
				sb.AppendLine("}");
				File.WriteAllText(path_output, sb.ToString(), Encoding.UTF8);
				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return;
			}
		}
		public class Importer : AssetPostprocessor
		{
			static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
				foreach (var i in importedAssets)
				{
					if (i.EndsWith("ja_word.txt"))
					{
						MakeLID();
					}
				}
			}
		}
	}
}