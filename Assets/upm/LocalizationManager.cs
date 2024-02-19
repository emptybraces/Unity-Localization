using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace EmptyBraces.Localization
{
	public static class LocalizationManager
	{
		public static readonly string k_SettingsFileName = "LocalizationSettings";
		public static string CurrentLoadedLaunguageId;
		static Dictionary<string, AsyncOperationHandle<TMP_FontAsset>> _cacheAASHandles = new();
		public static void Load(SystemLanguage language)
		{
			var lan_id = Settings.Instance.GetId(language);
			Assert.IsNotNull(lan_id, "unsuported language: " + language);
			if (CurrentLoadedLaunguageId == lan_id)
				return;
			bool failed = false;
			var path = Path.Combine(Settings.Instance.LocalizeFileLocation, $"{lan_id}_word.txt");
			failed = failed || !Word.LoadFromFile(path);
			if (failed)
			{
				lan_id = Settings.Instance.GetId(Application.systemLanguage);
				cn.logwf(path, " file loading failed, retry with OS language. ", lan_id);
				failed = false;
				path = Path.Combine(Settings.Instance.LocalizeFileLocation, $"{lan_id}_word.txt");
				failed = failed || !Word.LoadFromFile(path);
				if (failed)
				{
					cn.logef(path, " file loading failed");
					return;
				}
			}
			Release();
			CurrentLoadedLaunguageId = lan_id;
		}
		public static TMP_FontAsset LoadIfNeeded(string fontName, string lanId = null)
		{
			lanId ??= CurrentLoadedLaunguageId;
			var a_name = $"{fontName}_{lanId}";
			if (_cacheAASHandles.TryGetValue(a_name, out var op))
				return op.Status == AsyncOperationStatus.Succeeded ? op.Result : null;
			cn.logf(a_name);
			op = Addressables.LoadAssetAsync<TMP_FontAsset>(a_name);
			_cacheAASHandles[a_name] = op;
			op.WaitForCompletion();
			return op.Status == AsyncOperationStatus.Succeeded ? op.Result : null;
		}
		// public static Material LoadIfNeeded(string fontName, string lanId = null)
		// {
		// 	lanId ??= CurrentLoadedLaunguageId;
		// 	var a_name = $"{fontName}_{lanId}";
		// 	if (_cacheAASHandles.TryGetValue(a_name, out var op))
		// 		return op.Status == AsyncOperationStatus.Succeeded ? op.Result : null;
		// 	cn.logf(a_name);
		// 	op = Addressables.LoadAssetAsync<TMP_FontAsset>(a_name);
		// 	_cacheAASHandles[a_name] = op;
		// 	op.WaitForCompletion();
		// 	return op.Status == AsyncOperationStatus.Succeeded ? op.Result : null;
		// }

		public static void Release()
		{
			cn.logf();
			foreach (var i in _cacheAASHandles.Values)
			{
				Addressables.Release(i);
			}
			_cacheAASHandles.Clear();
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_cacheAASHandles = new();
		}
	}
}