using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace EmptyBraces.Localization
{
	public static class LocalizationManager
	{
		public const string k_AddressablesGroupName = "Localization";
		public const string k_SettingsFileName = "LocalizationSettings";
		public static string CurrentLoadedLaunguageId;
		static Dictionary<string, AsyncOperationHandle<IList<TMP_FontAsset>>> _cacheAASHandles = new();

		public static TMP_FontAsset LoadFontAssetIfNeeded(string fontName, string lanId = null)
		{
			if (_cacheAASHandles.TryGetValue(fontName, out var op))
				return op.Status == AsyncOperationStatus.Succeeded ? op.Result[0] : null;
			lanId ??= CurrentLoadedLaunguageId;
			cn.logf(fontName, lanId);
			var handle = Addressables.LoadAssetsAsync<TMP_FontAsset>((IEnumerable)new object[] { fontName, lanId }, null, Addressables.MergeMode.Intersection);
			_cacheAASHandles[fontName] = handle;
			handle.WaitForCompletion();
			return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result[0] : null;
		}

		public static void Release()
		{
			cn.logf();
			foreach (var i in _cacheAASHandles.Values)
				if (i.IsValid())
					Addressables.Release(i);
			_cacheAASHandles.Clear();
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_cacheAASHandles = new();
		}
	}
}