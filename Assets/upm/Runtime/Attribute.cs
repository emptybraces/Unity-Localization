using UnityEngine;
namespace Emptybraces.Localization
{
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public class SupportLanguagePopupAttribute : PropertyAttribute { }
	public class SupportedLanguageArrayAttribute : PropertyAttribute { }
	public class LIDAttribute : PropertyAttribute { }
	public class ReadOnlyAttribute : PropertyAttribute { }
}