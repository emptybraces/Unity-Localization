using System;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace EmptyBraces.Localization
{
	public class Settings : ScriptableObject
	{
		public SystemLanguage DefaultLanguage = SystemLanguage.English;
		[SerializeField] string _localizeFileLocation = "localization";
		public string LocalizeFileLocation => Path.Combine(Application.streamingAssetsPath, _localizeFileLocation);
		public string AutoGenerateLocalizeKeyFileLocation = "";
		public string AutoGenerateLocalizeKeyFileNamespace = "EmptyBraces.Localization";
		public bool EnableDebugLog = true;
		public SupportLanguage[] SupportLanguages;
		public FontAssetData[] SupportLanguageFontAssets;
		[System.Serializable]
		public class FontAssetData
		{
			public TMPro.TMP_FontAsset BaseFontAsset;
			[Tooltip("Set the FontAsset in order of SupportLanguage listitem.")]
#if UNITY_EDITOR
			[SupportedLanguageArray] public TMPro.TMP_FontAsset[] ActualFontAssets;
#endif
			[ReadOnly] public AssetReference[] ActualFontAssetRefs; // エディタ側で入れる
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			foreach (var i in SupportLanguageFontAssets)
			{
				if (i.ActualFontAssets.Length != i.ActualFontAssetRefs.Length)
				{
					Debug.Log("Detect array changed.");
					Array.Resize(ref i.ActualFontAssetRefs, i.ActualFontAssets.Length);
				}
				if (i.ActualFontAssets.Length != SupportLanguages.Length)
				{
					Debug.LogWarning("Need to set font assets as many as the languages you want to support.");
					break;
				}
			}
		}
#endif

		public static Settings Instance => _instance ??= Resources.Load<Settings>(LocalizationManager.k_SettingsFileName);
		static Settings _instance;

		[System.Serializable]
		public class SupportLanguage
		{
			public SystemLanguage Language;
			public string Prefix;
		}

		public int GetIndex(SystemLanguage lan)
		{
			for (int i = 0; i < SupportLanguages.Length; i++)
			{
				if (SupportLanguages[i].Language == lan)
					return i;
			}
			return -1;
		}
		public string GetPrefix(SystemLanguage lan)
		{
			foreach (var i in SupportLanguages)
				if (i.Language == lan)
					return i.Prefix;
			return null;
		}
		public string GetDefaultLaunguageId()
		{
			foreach (var i in SupportLanguages)
				if (i.Language == DefaultLanguage)
					return i.Prefix;
			return null;
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_instance = null;
		}
	}
}