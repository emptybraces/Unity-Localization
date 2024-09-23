using UnityEditor;
using UnityEngine;
namespace Emptybraces.Localization.Editor
{
	[CustomEditor(typeof(Sample))]
	internal class SampleInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (Application.isPlaying || UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings == null)
				return;
			GUILayout.Space(10);
			var sample = (Sample)target;
			var asset = sample._aRefDynamicTexts.editorAsset;
			if (asset != null && !asset.AddressableResourceExists())
			{
				asset.AddressableAddToGroup(UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.Name);
				Debug.Log("Add _aRefDynamicTexts to Addressables.");
			}
		}
	}
}
