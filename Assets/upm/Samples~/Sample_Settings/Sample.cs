using Emptybraces.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
namespace Emptybraces
{
	public class Sample : MonoBehaviour
	{
		[SerializeField] Toggle[] _languageToggles;
		[SerializeField] AssetReference _aRefDynamicTexts;
		[SerializeField] string _version = "1.0.0";
		GameObject _dynText;
		void Awake()
		{
			Word.LoadWordFile(SystemLanguage.English);
			
			_aRefDynamicTexts.InstantiateAsync(transform).Completed += handle =>
			{
				Debug.Log("Loaded DynamicTexts");
				_dynText = handle.Result;
				var texts = handle.Result.GetComponentsInChildren<TMP_Text>();
				texts[0].text = Word.Get(LID.com_yes);
				texts[1].text = Word.GetArray(LID.status_params)[2];
				if (texts[0].TryGetComponent(out TMProLocalize c) || (c = texts[0].gameObject.AddComponent<TMProLocalize>()))
					c.SetDynamicMessage(() => Word.Get(LID.com_yes));
				if (texts[1].TryGetComponent(out c) || (c = texts[1].gameObject.AddComponent<TMProLocalize>()))
					c.SetDynamicMessage(() => Word.GetArray(LID.status_params)[2]);
				if (texts[2].TryGetComponent(out c) || (c = texts[2].gameObject.AddComponent<TMProLocalize>()))
					c.SetDynamicMessage(() => Word.Get(LID.title_version, _version));
			};

			for (int i = 0; i < 3; ++i)
			{
				var icap = i;
				_languageToggles[i].onValueChanged.AddListener(isOn =>
				{
					if (isOn)
					{
						OnLoadLanguage(icap switch
						{
							0 => SystemLanguage.English,
							1 => SystemLanguage.Japanese,
							_ => SystemLanguage.ChineseSimplified,
						});
					}
				});
			}
		}

		void OnDestroy()
		{
			_aRefDynamicTexts.ReleaseInstance(_dynText);
		}

		public void OnLoadLanguage(SystemLanguage lan)
		{
			Word.LoadWordFile(lan);
			foreach (var i in FindObjectsByType<TMProLocalize>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				i.RefreshText();
		}
	}
}
