using System.IO;
using UnityEngine;
namespace EmptyBraces.Localization
{
	public class Settings : ScriptableObject
	{
		[SerializeField] string _localizeFileLocation = "localization";
		public string LocalizeFileLocation => Path.Combine(Application.streamingAssetsPath, _localizeFileLocation);
		public string SourceFileLocation = "";
		public SystemLanguage DefaultLanguage = SystemLanguage.English;
		public SupportLanguage[] SupportLanguages;
		public static Settings Instance => _instance ??= Resources.Load<Settings>(LocalizationManager.k_SettingsFileName);
		static Settings _instance;

		[System.Serializable]
		public class SupportLanguage
		{
			public SystemLanguage Language;
			public string Id;
		}

		public string GetId(SystemLanguage lan)
		{
			foreach (var i in SupportLanguages)
				if (i.Language == lan)
					return i.Id;
			return null;
		}
		public string GetDefaultLaunguageId()
		{
			foreach (var i in SupportLanguages)
				if (i.Language == DefaultLanguage)
					return i.Id;
			return null;
		}
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void _DomainReset()
		{
			_instance = null;
		}
	}
}