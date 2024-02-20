using EmptyBraces.Localization;
using UnityEngine;
namespace EmptyBraces
{
	public class Sample : MonoBehaviour
	{
		public SystemLanguage Language = SystemLanguage.English;
		public TMProLocalize _tmVersion;

		void Awake()
		{
			LocalizationManager.LoadWordFile(Language);
		}

		void Start()
		{
			_tmVersion.SetDynamicMessage(() => Word.Get(LID.title_version, "1.0"));
		}

		public void OnLoadLanguage(SystemLanguage lan)
		{
			LocalizationManager.LoadWordFile(Language);
			foreach (var i in FindObjectsByType<TMProLocalize>(FindObjectsSortMode.None))
				i.RefreshText();
		}
	}
}