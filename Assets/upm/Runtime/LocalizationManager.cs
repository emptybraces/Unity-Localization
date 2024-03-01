using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Emptybraces.Localization
{
	public static class LocalizationManager
	{
		public const string k_AddressablesGroupName = "Localization";
		public const string k_AddressablesLabelPrefix = "localization_";
		public const string k_SettingsFileName = "LocalizationSettings";
		public static SystemLanguage CurrentLoadedLaunguage = SystemLanguage.Unknown;
		static Dictionary<int, AsyncOperationHandle<TMP_FontAsset>> _cacheAASHandles = new();

		public static TMP_FontAsset LoadFontAssetIfNeeded(TMP_FontAsset fontAsset, SystemLanguage? language = null)
		{
			if (_cacheAASHandles.TryGetValue(fontAsset.GetInstanceID(), out var op))
				return op.Status == AsyncOperationStatus.Succeeded ? op.Result : null;
			language ??= CurrentLoadedLaunguage;
			var lan_idx = Settings.Instance.GetIndex(language.Value);
			Assert.IsFalse(lan_idx == -1, $"{language.Value} is not supported: ");
			if (Settings.Instance.EnableDebugLog)
				Debug.Log($"Load to cache: {fontAsset} of {language}");
			// var handle = Addressables.LoadAssetsAsync<TMP_FontAsset>((IEnumerable)new object[] { fontName, k_AddressablesLabelPrefix + lanId }, null, Addressables.MergeMode.Intersection);
			AsyncOperationHandle<TMP_FontAsset> handle;
			for (int i = 0; i < Settings.Instance.SupportLanguageFontAssets.Length; i++)
			{
				var item = Settings.Instance.SupportLanguageFontAssets[i];
				if (item.BaseFontAsset == fontAsset)
				{
					// nullがセットされていたら
					if (item.ActualFontAssetRefs[lan_idx].RuntimeKeyIsValid())
					{
						handle = item.ActualFontAssetRefs[lan_idx].LoadAssetAsync<TMP_FontAsset>();
						_cacheAASHandles[fontAsset.GetInstanceID()] = handle;
						handle.WaitForCompletion();
						if (Settings.Instance.EnableDebugLog)
							Debug.Log($"{lan_idx}, {handle.Result}");
						return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
					}
					return null;
				}
			}
			if (Settings.Instance.EnableDebugLog)
				Debug.LogError($"Failed to Load: {fontAsset} of {language}");
			return null;
		}

		public static void Release()
		{
			foreach (var i in _cacheAASHandles.Values)
			{
				if (i.IsValid())
					Addressables.Release(i);
			}
			_cacheAASHandles.Clear();
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_cacheAASHandles = new();
			CurrentLoadedLaunguage = SystemLanguage.Unknown;
		}
	}
}