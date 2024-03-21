using System;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Emptybraces.Localization
{
	public class Settings : ScriptableObject
	{
		public SystemLanguage DefaultLanguage = SystemLanguage.English;
		[SerializeField] string _localizeFileLocation = "localization";
		public string LocalizeFileLocation => Path.Combine(Application.streamingAssetsPath, _localizeFileLocation);
		public string AutoGenerateLocalizeKeyFileLocation = "";
		public string AutoGenerateLocalizeKeyFileNamespace = "Emptybraces.Localization";
		public bool EnableDebugLog = true;
		public SupportLanguage[] SupportLanguages = new SupportLanguage[0];
		public FontAssetData[] SupportLanguageFontAssets = new FontAssetData[0];
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
			if (Application.isPlaying)
				return;
			if (0 == SupportLanguageFontAssets.Length)
				return;
			using (ListPool<string>.Get(out var list))
			{
				foreach (var i in SupportLanguageFontAssets)
				{
					if (i.ActualFontAssets.Length != i.ActualFontAssetRefs.Length)
					{
						Debug.Log("[LocalizationManager] Detect array changed.");
						Array.Resize(ref i.ActualFontAssetRefs, i.ActualFontAssets.Length);
					}
					if (i.ActualFontAssets.Length != SupportLanguages.Length)
					{
						Debug.LogWarning("[LocalizationManager] Need to set font assets as many as the languages you want to support.");
						break;
					}
					if (list.Contains(i.BaseFontAsset.name))
					{
						Debug.LogWarning("[LocalizationManager] Not possible to store assets with the same name in the BaseFontAsset field because of the use of name as the cache key.");
						break;
					}
					list.Add(i.BaseFontAsset.name);
				}
			}
		}
#endif
		// Resources管理やめた。
		// ビルド時にSettings内で参照するベースフォントアセットは、Resourcesが管轄する領域にでき、Addressables側で参照しているベースフォントアセットと
		// 異なるものになってしまう。どうせコピーができるなら、最善な方法としてコピーを発生しなくさせる方に持っていくともできるAddressables側管理に変更。
		// https://docs.unity3d.com/Packages/com.unity.addressables@2.0/manual/AssetDependencies.html

		// public static Settings Instance => _instance ??= Resources.Load<Settings>(LocalizationManager.k_SettingsFileName);
		public static Settings Instance
		{
			get
			{
				if (_instance == null)
				{
					if (Application.isPlaying)
					{
						_handleSettings = Addressables.LoadAssetAsync<Settings>(LocalizationManager.k_SettingsFileName);
						_handleSettings.WaitForCompletion();
						Assert.IsTrue(_handleSettings.Status == AsyncOperationStatus.Succeeded, "[LocalizationManager] Failed to load LocalizationSettings.");
						Debug.Log("[LocalizationManager] Loaded LocalizationManager Settings.");
						_instance = _handleSettings.Result;
						if (_instance != null)
						{
							Application.quitting += () =>
							{
								if (_handleSettings.IsValid())
									Addressables.Release(_handleSettings);
							};
						}
					}
#if UNITY_EDITOR
					else
					{
						Editor_Load(out _instance);
					}
#endif
				}
				return _instance;
			}
		}
#if UNITY_EDITOR
		public static bool Editor_Load(out Settings settings)
		{
			settings = null;
			var guids = UnityEditor.AssetDatabase.FindAssets("t:Emptybraces.Localization.Settings", null);
			if (guids.Length == 0)
				Debug.LogWarning("[LocalizationManager] Please make the Localization settings asset. See in menu 'Assets/Localization//Create Localization Settings'");
			else
				settings = UnityEditor.AssetDatabase.LoadAssetAtPath<Settings>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
			return settings != null;
		}
#endif

		static Settings _instance;
		static AsyncOperationHandle<Settings> _handleSettings;

		[System.Serializable]
		public class SupportLanguage
		{
			public SystemLanguage Language;
			public string Prefix;
			public override string ToString()
			{
				return Language + Prefix;
			}
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
