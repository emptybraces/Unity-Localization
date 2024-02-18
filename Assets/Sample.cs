using System.Collections.Generic;
using UnityEngine;
namespace EmptyBraces
{
	public class Sample : MonoBehaviour
	{
		public SystemLanguage Language = SystemLanguage.English;

		void Awake()
		{
			LocalizationManager.Load(Language);
		}

		public void OnLoadLanguage(SystemLanguage lan)
		{
			LocalizationManager.Load(Language);
			foreach (var i in FindObjectsByType<Localization.TMProLocalize>(FindObjectsSortMode.None))
				i.RefreshText();
		}
	}
}