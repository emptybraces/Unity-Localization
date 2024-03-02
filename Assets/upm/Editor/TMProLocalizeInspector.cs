using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
namespace Emptybraces.Localization.Editor
{
	[CustomEditor(typeof(TMProLocalize))]
	public class TMProLocalizeInspector : UnityEditor.Editor
	{
		SerializedProperty _key;
		static SystemLanguage _language = SystemLanguage.English;
		void OnEnable()
		{
			_key = serializedObject.FindProperty(nameof(TMProLocalize.Key));
		}
		public void OnSceneGUI()
		{
			Handles.Label(((TMProLocalize)target).transform.position, _key.stringValue);
		}
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUILayout.Space(10);
			var tmpro_localize = (TMProLocalize)target;
			using (var h = new GUILayout.HorizontalScope())
			{
				var displays = Settings.Instance.SupportLanguages.Select(e => e.Language).ToArray();
				int idx = Mathf.Max(0, Array.IndexOf(displays, _language));
				idx = EditorGUILayout.Popup(idx, displays.Select(e => e.ToString()).ToArray());
				_language = displays[idx];
				// _language = (SystemLanguage)EditorGUILayout.EnumPopup("Load Language", _language);
				if (GUILayout.Button("Load"))
				{
					_ResetWithoutText();
					Word.LoadWordFile(_language);
					tmpro_localize.RefreshText();
					EditorApplication.QueuePlayerLoopUpdate();
				}
			}
			if (GUILayout.Button("Load Next Language"))
			{
				_ResetWithoutText();
				var idx = Array.FindIndex(Settings.Instance.SupportLanguages, e => e.Language == _language);
				idx = (int)Mathf.Repeat(idx + 1, Settings.Instance.SupportLanguages.Length);
				_language = Settings.Instance.SupportLanguages[idx].Language;
				Word.LoadWordFile(_language);
				tmpro_localize.RefreshText();
				EditorApplication.QueuePlayerLoopUpdate();
			}
			if (GUILayout.Button("Load Next Language(Whole Scene)"))
			{
				_ResetWithoutText();
				var idx = Array.FindIndex(Settings.Instance.SupportLanguages, e => e.Language == _language);
				idx = (int)Mathf.Repeat(idx + 1, Settings.Instance.SupportLanguages.Length);
				_language = Settings.Instance.SupportLanguages[idx].Language;
				var static_tm = new List<TMPro.TMP_Text>();
				foreach (var i in FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				{
					if (!i.TryGetComponent<TMProLocalize>(out _))
					{
						var key = Word.Data.FirstOrDefault(e => e.Value as string == i.text).Key;
						if (null != key)
						{
							static_tm.Add(i);
							i.text = key;
						}
					}
				}
				Word.LoadWordFile(_language);
				foreach (var i in FindObjectsByType<TMProLocalize>(FindObjectsInactive.Include, FindObjectsSortMode.None))
					i.RefreshText();
				foreach (var i in static_tm)
					i.text = Word.Get(i.text);
				EditorApplication.QueuePlayerLoopUpdate();
			}
			if (GUILayout.Button("Reset"))
			{
				_Reset();
				EditorApplication.QueuePlayerLoopUpdate();
			}
		}
		void _Reset()
		{
			var tmpro_localize = (TMProLocalize)target;
			tmpro_localize.GetComponent<TMPro.TMP_Text>().text = "";
			foreach (var i in tmpro_localize.GetComponentsInChildren<TMPro.TMP_SubMeshUI>())
			{
				DestroyImmediate(i.gameObject);
			}
		}
		void _ResetWithoutText()
		{
			var tmpro_localize = (TMProLocalize)target;
			var c = tmpro_localize.GetComponent<TMPro.TMP_Text>();
			var text = c.text;
			c.text = "";
			foreach (var i in tmpro_localize.GetComponentsInChildren<TMPro.TMP_SubMeshUI>())
				DestroyImmediate(i.gameObject);
			c.text = text;
		}
	}
}