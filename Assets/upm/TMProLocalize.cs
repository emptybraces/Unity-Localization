using System;
using System.Collections.Generic;
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
		[ValueDropdown("@GetLocalizeKeyDropDownList()", ExpandAllMenuItems = true, FlattenTreeView = true)] 
#else
		[LID]
#endif
		public string Key;
		[Min(-1)] public int Index = -1;
		Func<string> _dynamicMessage;
		static bool _isRegisterQuitting;
#if UNITY_EDITOR
		static List<TMPro.TMP_FontAsset> _fontAssets = new();
#endif
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
			var tm = GetComponent<TMPro.TMP_Text>();
			if (tm.font.fallbackFontAssetTable.Count == 0)
			{
				tm.font.fallbackFontAssetTable.Add(LocalizationManager.LoadFontAssetIfNeeded(tm.font.name));
			}
			else
			{
				tm.font.fallbackFontAssetTable[0] = LocalizationManager.LoadFontAssetIfNeeded(tm.font.name);
			}
			tm.font.ReadFontAssetDefinition();
			if (_dynamicMessage != null)
			{
				tm.text = _dynamicMessage();
				return;
			}
			// add componentしたときはdataはnull
			if (!string.IsNullOrEmpty(Key))
			{
				if (0 <= Index)
					tm.text = Word.GetArray(Key)[Index];
				else
					tm.text = Word.Get(Key);
			}
		}
		public void SetDynamicMessage(Func<string> cb)
		{
			_dynamicMessage = cb;
			if (cb != null)
			{
				var tm = GetComponent<TMPro.TMP_Text>();
				tm.text = cb();
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
