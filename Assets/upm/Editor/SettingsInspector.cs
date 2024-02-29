using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
namespace EmptyBraces.Localization.Editor
{
	[CustomEditor(typeof(Settings))]
	public class SettingsInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.LabelField("Helper Menu:", EditorStyles.boldLabel);
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
				var aas_settings = AddressableAssetSettingsDefaultObject.Settings;
				// 最初に全てのアドレスを消す
				var group = aas_settings.FindGroup(LocalizationManager.k_AddressablesGroupName);
				if (group)
				{
					foreach (var i in group.entries.ToArray())
					{
						group.RemoveAssetEntry(i);
					}
				}
				// Addresasblesの登録と、ActualFontAssetRefsを埋める
				foreach (var item in settings.SupportLanguageFontAssets)
				{
					for (int i = 0; i < item.ActualFontAssets.Length; ++i)
					{
						var j = item.ActualFontAssets[i];
						// AssetRefrenceを埋める
						item.ActualFontAssetRefs[i].SetEditorAsset(j);
						if (j != null)
						{
							var entry = j.SetAddressableGroup(LocalizationManager.k_AddressablesGroupName);
							entry.SetAddress(j.name);
							// bundletypeでラベルごとにまとめるので必要
							var label = LocalizationManager.k_AddressablesLabelPrefix + settings.SupportLanguages[i].Prefix;
							aas_settings.AddLabel(label);
							entry.SetLabel(label, true);
						}
					}
				}
				// ラベルごとにまとめる
				group = aas_settings.FindGroup(LocalizationManager.k_AddressablesGroupName);
				var schema = group.GetSchema<BundledAssetGroupSchema>();
				schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
			}

			if (GUILayout.Button("Create LID.cs"))
			{
				Menu.CreateLID();
			}
			// if (GUILayout.Button("Test"))
			// {
			// 	Debug.Log(settings.SupportLanguageFontAssets[0].ActualFontAssetRef.RuntimeKey);
			// 	Debug.Log(settings.SupportLanguageFontAssets[0].ActualFontAssetRef.Asset);
			// 	Debug.Log(settings.SupportLanguageFontAssets[0].ActualFontAssetRef.AssetGUID);
			// 	Debug.Log(settings.SupportLanguageFontAssets[0].ActualFontAssetRef.editorAsset);
			// }
		}
	}
}