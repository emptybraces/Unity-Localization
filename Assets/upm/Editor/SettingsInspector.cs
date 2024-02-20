using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
namespace EmptyBraces.Localization.Editor
{
	[CustomEditor(typeof(Settings))]
	public class SettingsInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var settings = target as Settings;
			int error_no = 0;
			foreach (var i in settings.SupportLanguageFontAssets)
			{
				if (i.ActualFontAssets.Length != settings.SupportLanguages.Length)
				{
					error_no = 1;
					break;
				}
			}
			GUI.enabled = 0 == error_no;
			switch (error_no)
			{
				case 1: EditorGUILayout.HelpBox("FontAsset must be setup for all supported languages.", MessageType.Error); break;
			}
			if (AddressableAssetSettingsDefaultObject.Settings == null)
			{
				EditorGUILayout.HelpBox("Need Addressable settings .", MessageType.Error);
				GUI.enabled = false;
			}
			if (GUILayout.Button("Register above FontAsset to Addressables"))
			{
				var addr = AddressableAssetSettingsDefaultObject.Settings;
				// グループは消してから作る
				addr.RemoveGroup(addr.FindGroup(LocalizationManager.k_AddressablesGroupName));
				foreach (var FontAsset in settings.SupportLanguageFontAssets)
				{
					for (int i = 0; i < FontAsset.ActualFontAssets.Length; ++i)
					{
						var j = FontAsset.ActualFontAssets[i];
						if (j != null)
						{
							var entry = j.SetAddressableGroup("Localization");
							entry.SetAddress(FontAsset.MediateFontAsset.name);
							entry.SetLabel(settings.SupportLanguages[i].Id, true);
						}
					}
				}
			}
		}
	}
}