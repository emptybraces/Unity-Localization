using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
namespace EmptyBraces.Localization.Editor
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
				_language = (SystemLanguage)EditorGUILayout.EnumPopup("Load Language", _language);
				if (GUILayout.Button("Load"))
				{
					_Reset();
					LocalizationManager.Load(_language);
					tmpro_localize.RefreshText();
					EditorApplication.QueuePlayerLoopUpdate();
				}
			}
			if (GUILayout.Button("Load Next Language"))
			{
				_Reset();
				var idx = Array.FindIndex(Settings.Instance.SupportLanguages, e => e.Language == _language);
				idx = (int)Mathf.Repeat(idx + 1, Settings.Instance.SupportLanguages.Length);
				_language = Settings.Instance.SupportLanguages[idx].Language;
				LocalizationManager.Load(_language);
				tmpro_localize.RefreshText();
				EditorApplication.QueuePlayerLoopUpdate();
			}
			// if (GUILayout.Button("Set key to body"))
			// {
			// 	var c = (TMProLocalize)target;
			// 	c.GetComponent<TMPro.TMP_Text>().text = c.Key;
			// 	EditorApplication.QueuePlayerLoopUpdate();
			// }
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
	}
}