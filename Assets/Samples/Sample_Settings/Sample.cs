using Emptybraces.Localization;
using UnityEngine;
using UnityEngine.UI;
namespace Emptybraces
{
	public class Sample : MonoBehaviour
	{
		[SerializeField] TMProLocalize _tmVersion;
		[SerializeField] Toggle[] _languageToggles;
		[SerializeField] string _version = "1.0.0";
		void Awake()
		{
			Word.LoadWordFile(SystemLanguage.English);
			for (int i = 0; i < 3; ++i)
			{
				var icap = i;
				_languageToggles[i].onValueChanged.AddListener(isOn =>
				{
					if (isOn)
					{
						OnLoadLanguage(icap switch {
							0 => SystemLanguage.English,
							1 => SystemLanguage.Japanese,
							_ => SystemLanguage.ChineseSimplified,
						});
					}
				});
			}
		}

		void Start()
		{
			_tmVersion.SetDynamicMessage(() => Word.Get(LID.title_version, _version));
		}

		public void OnLoadLanguage(SystemLanguage lan)
		{
			Word.LoadWordFile(lan);
			foreach (var i in FindObjectsByType<TMProLocalize>(FindObjectsInactive.Include, FindObjectsSortMode.None))
				i.RefreshText();
		}
	}
}