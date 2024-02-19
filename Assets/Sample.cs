using UnityEngine;
namespace EmptyBraces
{
	public class Sample : MonoBehaviour
	{
		public SystemLanguage Language = SystemLanguage.English;

		void Awake()
		{
			Localization.LocalizationManager.Load(Language);
		}

		public void OnLoadLanguage(SystemLanguage lan)
		{
			Localization.LocalizationManager.Load(Language);
			foreach (var i in FindObjectsByType<Localization.TMProLocalize>(FindObjectsSortMode.None))
				i.RefreshText();
		}
	}
}