using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EmptyBraces.Localization
{
	public static class Word
	{
		static string[] k_Split = new[] { "\t", " " };
		public static Dictionary<string, object> Data = new(256);
		static List<string> _tmpStringList = new(128);
		public static bool LoadFromFile(string path)
		{
			try
			{
				if (!File.Exists(path))
				{
					cn.loge("File not found. path is", path);
					return false;
				}
				var text = File.ReadAllText(path, System.Text.Encoding.UTF8);
				Load(text);
				cn.logf("Complete load.", path);
			}
			catch (Exception e)
			{
				// Dialogue.Confirm1("Fatal", "Exception occured please check the log file.").Forget();
				Debug.LogError("Exception: " + e.Message);
				return false;
			}
			return true;
		}

		public static bool Load(string textData)
		{
			if (string.IsNullOrEmpty(textData))
			{
				cn.loge("Text data is empty.");
				return false;
			}
			Data.Clear();
			_tmpStringList.Clear();
			var span_text = textData.AsSpan();
			var s_idx = 0;
			var line_no = 0;
			string last_key = "";
			while (s_idx < span_text.Length)
			{
				// セパレータで区切られた１カラムを探す
				var e_idx = span_text.IndexOf(Environment.NewLine);
				// 最後のカラムを処理するためにendIndexの操作
				if (e_idx == -1)
					e_idx = span_text.Length;
				// 部分文字列をReadOnlySpan<char>で受け取る
				var line = span_text[s_idx..e_idx];
				// 次のカラムを探すためs_idx更新
				if (span_text.Length != e_idx)
				{
					span_text = span_text[(e_idx + Environment.NewLine.Length)..];
				}
				// s_idx = e_idx + 2; /*改行コードは2charある様子*/

				++line_no;
				if (line.TrimStart().StartsWith("/") || line.IsEmpty)
					continue;
				// keyの検出
				var key_idx = line.IndexOfAny('\t', ' ');
				// Assert.IsFalse(key_idx == -1);
				// 先頭がタブまたは空白の場合は配列型
				if (key_idx == 0)
				{
					// keyは直前のものを使用し、valueを検出する
					_tmpStringList.Add(line.Trim().ToString());
				}
				// keyとvalueが一対一のもの
				else
				{
					// 直前が配列終わりだった場合、配列を格納する
					_AddWordArray(last_key, _tmpStringList);

					// セパレータがないなら、キー＋空欄で登録する。
					var key = key_idx == -1 ? line : line[..key_idx];
					last_key = key.ToString();
					// cn.logBlue(key_idx, last_key);

					// valueの検出
					string value = ""; // セパレータがないなら、キー＋空欄で登録する。
					if (key_idx != -1)
					{
						var value_span = key_idx == -1 ? "" : line[(key_idx + 1)..].Trim();
						value = value_span.ToString();
					}
					// 格納
					Data.Add(last_key, value);
					cn.log($"Add | K={last_key}, V={value}");
				}
			}
			// 直前が配列終わりだった場合、配列を格納する
			_AddWordArray(last_key, _tmpStringList);
			return true;
		}
		static void _AddWordArray(string lastKey, List<string> values)
		{
			if (0 < values.Count)
			{
				values.Insert(0, (string)Data[lastKey]); // 先頭のキーを配列の先頭に設定する。
				Data[lastKey] = values.ToArray(); // 上書きする。
				cn.log($"AddArray | K={lastKey}, V={string.Join(",", values)}");
				values.Clear();
			}
		}

		static string[] _error;
		static string[] Error => _error ??= new string[2] { "unknown id", "unknown id" };
		public static string Get(string id) => Data.TryGetValue(id, out var s) ? (string)s : $"unknown id:{id}";
		public static string Get<T>(string id, T t1) => Data.TryGetValue(id, out var s) ? string.Format((string)s, t1) : $"unknown id:{id}";
		public static string Get<T1, T2>(string id, T1 t1, T2 t2) => Data.TryGetValue(id, out var s) ? string.Format((string)s, t1, t2) : $"unknown id:{id}";
		public static string Get<T1, T2, T3>(string id, T1 t1, T2 t2, T3 t3) => Data.TryGetValue(id, out var s) ? string.Format((string)s, t1, t2, t3) : $"unknown id:{id}";
		public static string Get(string id, params object[] args) => Data.TryGetValue(id, out var s) ? string.Format((string)s, args) : $"unknown id:{id}";
		public static string[] GetArray(string id) => Data.TryGetValue(id, out var s) ? (string[])s : Error;
		// -------------
		static object[] _a = new object[9];
		public static string Get<T1, T2, T3, T4>(string id, T1 t1, T2 t2, T3 t3, T4 t4) => Data.TryGetValue(id, out var s) ? string.Format((string)s, _Format(t1, t2, t3, t4)) : $"unknown id:{id}";
		public static string Get<T1, T2, T3, T4, T5>(string id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => Data.TryGetValue(id, out var s) ? string.Format((string)s, _Format(t1, t2, t3, t4, t5)) : $"unknown id:{id}";
		public static string Get<T1, T2, T3, T4, T5, T6>(string id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => Data.TryGetValue(id, out var s) ? string.Format((string)s, _Format(t1, t2, t3, t4, t5, t6)) : $"unknown id:{id}";
		public static string Get<T1, T2, T3, T4, T5, T6, T7>(string id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => Data.TryGetValue(id, out var s) ? string.Format((string)s, _Format(t1, t2, t3, t4, t5, t6, t7)) : $"unknown id:{id}";
		public static string Get<T1, T2, T3, T4, T5, T6, T7, T8>(string id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => Data.TryGetValue(id, out var s) ? string.Format((string)s, _Format(t1, t2, t3, t4, t5, t6, t7, t8)) : $"unknown id:{id}";
		public static string Get<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string id, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => Data.TryGetValue(id, out var s) ? string.Format((string)s, _Format(t1, t2, t3, t4, t5, t6, t7, t8, t9)) : $"unknown id:{id}";
		static object[] _Format<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3) { _a[0] = t0; _a[1] = t1; _a[2] = t2; _a[3] = t3; return _a; }
		static object[] _Format<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4) { _a[0] = t0; _a[1] = t1; _a[2] = t2; _a[3] = t3; _a[4] = t4; return _a; }
		static object[] _Format<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) { _a[0] = t0; _a[1] = t1; _a[2] = t2; _a[3] = t3; _a[4] = t4; _a[5] = t5; return _a; }
		static object[] _Format<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) { _a[0] = t0; _a[1] = t1; _a[2] = t2; _a[3] = t3; _a[4] = t4; _a[5] = t5; _a[6] = t6; return _a; }
		static object[] _Format<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) { _a[0] = t0; _a[1] = t1; _a[2] = t2; _a[3] = t3; _a[4] = t4; _a[5] = t5; _a[6] = t6; _a[7] = t7; return _a; }
		static object[] _Format<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) { _a[0] = t0; _a[1] = t1; _a[2] = t2; _a[3] = t3; _a[4] = t4; _a[5] = t5; _a[6] = t6; _a[7] = t7; _a[8] = t8; return _a; }
#if ODIN_INSPECTOR
		public static IEnumerable GetLocalizeKeyDropDownList()
		{
			if (LoadFromFile($"{Localization.ROOT_PATH}/en_word.txt").Forget()) {

			}
			var list = new ValueDropdownList<string> { "" };
			foreach (var kvp in Data)
			{
				if (kvp.Value is string value)
					list.Add($"{kvp.Key} | {value.Replace("</", "<?").Replace("\n", "")}", kvp.Key);
				else if (kvp.Value is string[] values)
				{
					list.Add($"{kvp.Key} | {string.Concat(values.Select(e => e.Replace("</", "<?").Replace("\n", "") + ","))}", kvp.Key);
				}
			}
			return list;
		}
#endif
#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			Data.Clear();
		}
#endif
	}
}