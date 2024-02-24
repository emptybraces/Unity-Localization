using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EmptyBraces.Localization.Editor
{
	[CustomPropertyDrawer(typeof(SupportLanguagePopupAttribute))]
	public class SupportLanguagePopupAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Enum)
				return;
			if (Settings.Instance == null)
				return;
			var displays = Settings.Instance.SupportLanguages.Select(e => e.Language).ToArray();
			int idx = Mathf.Max(0, Array.IndexOf(displays, property.enumValueIndex));
			using (new EditorGUI.PropertyScope(position, label, property))
			{
				EditorGUI.LabelField(position, label);
				position.x += EditorGUIUtility.labelWidth;
				position.xMax -= EditorGUIUtility.labelWidth;
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					idx = EditorGUI.Popup(position, idx, displays.Select(e => e.ToString()).ToArray());
					if (scope.changed)
					{
						for (int i = 0, l = Enum.GetValues(typeof(SystemLanguage)).Length; i < l; ++i)
						{
							if ((SystemLanguage)i == displays[idx])
							{
								property.enumValueIndex = i;
								break;
							}
						}
					}
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(SupportedLanguageArrayAttribute))]
	public class SupportedLanguageArrayAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			try
			{
				var s = property.propertyPath[^5..];
				var n = System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value;
				var pos = int.Parse(n, System.Globalization.NumberStyles.Integer);
				if (pos < Settings.Instance.SupportLanguages.Length)
					EditorGUI.PropertyField(position, property, new GUIContent(Settings.Instance.SupportLanguages[pos].Language.ToString()));
				else
					EditorGUI.PropertyField(position, property, label);
			}
			catch
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}
	}

	[CustomPropertyDrawer(typeof(LIDAttribute))]
	public class LIDAttributeDrawer : PropertyDrawer
	{
		static string[] _displays;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			using (new EditorGUI.PropertyScope(position, label, property))
			{
				EditorGUI.LabelField(position, label);
				position.x += EditorGUIUtility.labelWidth;
				position.xMax -= EditorGUIUtility.labelWidth;
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var field_width = position.width;
					position.xMax = position.x + field_width * 0.5f;
					_FetchLIDClassField();
					var compare = property.stringValue + " |";
					var idx = Array.FindIndex(_displays, e => e.StartsWith(compare, StringComparison.Ordinal));
					idx = EditorGUI.Popup(position, idx, _displays);
					if (scope.changed)
						property.stringValue = _displays[idx].Split(" | ")[0];

					position.x = position.xMax;
					position.xMax = position.x + field_width * 0.5f;
					EditorGUI.PropertyField(position, property, GUIContent.none);
				}
			}
		}

		void _FetchLIDClassField()
		{
			if (_displays != null)
				return;
			try
			{
				if (Settings.Instance != null)
					Word.LoadWordFile(Settings.Instance.DefaultLanguage);
				else
				{
					Debug.LogWarning("Not found LocalizationSettings, please create it.");
					_displays = new string[1] { "Not found LocalizationSettings, please create it." };
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				_displays = new string[1] { "Exception orrured, see console." };
			}
			try
			{

				Type type = null;
				var assembly = Assembly.Load("Assembly-CSharp");
				if (assembly != null)
				{
					var typename = !string.IsNullOrEmpty(Settings.Instance.AutoGenerateLocalizeKeyFileNamespace)
						? $"{Settings.Instance.AutoGenerateLocalizeKeyFileNamespace}.LID" : "LID";
					type = assembly.GetType(typename);
					if (type == null)
					{
						Debug.LogWarning("Not found Lid.cs, please create it.");
						_displays = new string[1] { "Not found Lid.cs, please create it." };
					}
				}
				// Debug.Log(type);
				if (type != null)
				{
					_displays = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
						.Where(e => e.IsLiteral)
						.Select(e =>
						{
							var key = (string)e.GetRawConstantValue();
							if (Word.Data == null)
								return key;
							if (Word.Data.TryGetValue(key, out var value))
								if (value is string s)
									return $"{key} | {Regex.Replace(s, "<.*?>", "")}";
								else if (value is string[] sa)
									return $"{key} | {Regex.Replace(string.Join(",", sa), "<.*?>", "")}";
							return key;
						})
						.ToArray();
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				_displays = new string[1] { "Exception orrured, see console." };
			}
		}
	}
}