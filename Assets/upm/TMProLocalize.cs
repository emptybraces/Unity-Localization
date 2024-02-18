using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace EmptyBraces.Localization
{
	[RequireComponent(typeof(TMPro.TMP_Text))]
	public class TMProLocalize : MonoBehaviour
	{
#if ODIN_INSPECTOR
		[ValueDropdown("@Word.GetLocalizeKeyDropDownList()", ExpandAllMenuItems = true, FlattenTreeView = true)] 
#endif
		public string Key;
		public int Index = -1;
		public Func<string> DynamicMessage;
		static bool _isRegisterQuitting;
#if UNITY_EDITOR
		static List<TMP_FontAsset> _fontAssets = new();
#endif
		public Material mat;

		void Start()
		{
#if UNITY_EDITOR
			// プレイ終了時にクリアする。
			var font = GetComponent<TMPro.TMP_Text>().font;
			if (!_fontAssets.Contains(font))
			{
				_fontAssets.Add(font);
				Application.quitting += () =>
				{
					cn.log("Clear font fallback list", font);
					font.fallbackFontAssetTable.Clear();
				};

			}
#endif
			if (!_isRegisterQuitting)
			{
				_isRegisterQuitting = true;
				Application.quitting += () =>
				{
					LocalizationManager.Release();
				};
			}
			RefreshText();
		}
		public void RefreshText()
		{
			var tmpro = GetComponent<TMPro.TMP_Text>();
			if (tmpro.font.fallbackFontAssetTable.Count == 0)
			{
				tmpro.font.fallbackFontAssetTable.Add(LocalizationManager.LoadIfNeeded(tmpro.font.name));
			}
			else
			{
				tmpro.font.fallbackFontAssetTable[0] = LocalizationManager.LoadIfNeeded(tmpro.font.name);
			}
			tmpro.font.ReadFontAssetDefinition();
			if (DynamicMessage != null)
			{
				tmpro.text = DynamicMessage();
				return;
			}
			// add componentしたときはdataはnull
			if (!string.IsNullOrEmpty(Key))
			{
				if (0 <= Index)
					tmpro.text = Word.GetArray(Key)[Index];
				else
					tmpro.text = Word.Get(Key);
			}
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_isRegisterQuitting = false;
			_fontAssets = new();
		}
	}
}
