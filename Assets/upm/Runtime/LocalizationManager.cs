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
		static Dictionary<string, AsyncOperationHandle<TMP_FontAsset>> _cacheAASHandles = new();

		public static TMP_FontAsset LoadFontAssetIfNeeded(TMP_FontAsset hasSetFontAssetOnInspector, SystemLanguage? language = null)
		{
			var cached_key = hasSetFontAssetOnInspector.name;
			if (_cacheAASHandles.TryGetValue(cached_key, out var op))
				return op.Status == AsyncOperationStatus.Succeeded ? op.Result : null;
			language ??= CurrentLoadedLaunguage;
			var lan_idx = Settings.Instance.GetIndex(language.Value);
			Assert.IsFalse(lan_idx == -1, $"[LocalizationManager] {language.Value} is not supported: ");
			if (Settings.Instance.EnableDebugLog)
				Debug.Log($"[LocalizationManager] Load to cache: {hasSetFontAssetOnInspector} of {language}");
			// var handle = Addressables.LoadAssetsAsync<TMP_FontAsset>((IEnumerable)new object[] { fontName, k_AddressablesLabelPrefix + lanId }, null, Addressables.MergeMode.Intersection);
			for (int i = 0; i < Settings.Instance.SupportLanguageFontAssets.Length; i++)
			{
				var item = Settings.Instance.SupportLanguageFontAssets[i];
				if (item.BaseFontAsset == null)
					continue;
				// シーン内から参照しているフォントアセットと、Addressablesプレハブ内で参照しているフォントアセットは違うので、nameで一致検出
				if (item.BaseFontAsset.name == hasSetFontAssetOnInspector.name)
				{
					// ActualFontAssetにnullがセットされていない場合のみ
					if (item.ActualFontAssetRefs[lan_idx].RuntimeKeyIsValid())
					{
						if (Settings.Instance.EnableDebugLog)
							Debug.Log($"[LocalizationManager] Try load TMP_FontAsset. {language.Value}");
						var handle = item.ActualFontAssetRefs[lan_idx].LoadAssetAsync<TMP_FontAsset>();
						_cacheAASHandles[cached_key] = handle;
						handle.WaitForCompletion();
						if (Settings.Instance.EnableDebugLog)
							Debug.Log($"[LocalizationManager] Load complete TMP_FontAsset. {language.Value}, {handle.Result}, cache key: {cached_key}");
						return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
					}
					return null;
				}
			}
			if (Settings.Instance.EnableDebugLog)
				Debug.LogError($"[LocalizationManager] Failed to Load: {hasSetFontAssetOnInspector} of {language}");
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

		internal static void AddFallbackFont(TMP_FontAsset font)
		{
			if (font.fallbackFontAssetTable.Count == 0)
				font.fallbackFontAssetTable.Add(LoadFontAssetIfNeeded(font));
			else
				font.fallbackFontAssetTable[0] = LoadFontAssetIfNeeded(font);
			if (font.fallbackFontAssetTable[0] != null)
			{
				var baseinfo = font.faceInfo;
				var fallback_info = font.fallbackFontAssetTable[0].faceInfo;
				baseinfo.lineHeight = fallback_info.lineHeight;
				baseinfo.underlineOffset = fallback_info.underlineOffset;
				baseinfo.underlineThickness = fallback_info.underlineThickness;
				baseinfo.strikethroughOffset = fallback_info.strikethroughOffset;
				baseinfo.strikethroughThickness = fallback_info.strikethroughThickness;
				baseinfo.superscriptOffset = fallback_info.superscriptOffset;
				baseinfo.superscriptSize = fallback_info.superscriptSize;
				baseinfo.subscriptOffset = fallback_info.subscriptOffset;
				baseinfo.subscriptSize = fallback_info.subscriptSize;
				baseinfo.tabWidth = fallback_info.tabWidth;
				font.faceInfo = baseinfo;
			}
			font.ReadFontAssetDefinition();
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_cacheAASHandles = new();
			CurrentLoadedLaunguage = SystemLanguage.Unknown;
		}
	}
}
