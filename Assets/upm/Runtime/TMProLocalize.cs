using System;
using System.Collections.Generic;
using UnityEngine;
namespace Emptybraces.Localization
{
	[RequireComponent(typeof(TMPro.TMP_Text))]
	public class TMProLocalize : MonoBehaviour
	{
		[LID] public string Key;
		[Min(-1)] public int ArrayIndex = -1;
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
					Debug.Log($"Clear font fallback list. {font}");
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
				tm.font.fallbackFontAssetTable.Add(LocalizationManager.LoadFontAssetIfNeeded(tm.font));
			}
			else
			{
				tm.font.fallbackFontAssetTable[0] = LocalizationManager.LoadFontAssetIfNeeded(tm.font);
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
				if (0 <= ArrayIndex)
					tm.text = Word.GetArray(Key)[ArrayIndex];
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
#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_isRegisterQuitting = false;
			_fontAssets = new();
		}
#endif
	}
}
