using System;
using UnityEngine;
using UnityEditor;

namespace AutoDarkTheme
{
	class UserPreferences : SettingsProvider
	{
		static readonly string KEY_EDITORPREF_IS_ENABLED = "AutoDarkTheme.IsEnabled";

		public enum Mode
        {
			System,
			Time
        }

		private static bool s_prefsLoaded = false;

		private static bool s_isEnabled;
		private static Mode s_mode;

		internal class Styles
		{
			public static readonly GUIContent enabled = EditorGUIUtility.TrTextContent("Enabled");
		}

		public static event EventHandler PreferencesChanged;
		private static void RaisePreferencesChanged()
        {
			var handler = PreferencesChanged;
			handler?.Invoke(typeof(UserPreferences), EventArgs.Empty);
        }

		public static bool IsEnabled
		{
			get
			{
				LoadAllPreferenceValues();
				return s_isEnabled;
			}
			set
			{
				s_isEnabled = value;
				SaveAllPreferenceValues();
			}
		}

		private static void LoadAllPreferenceValues()
		{
			if (!s_prefsLoaded)
			{
				s_isEnabled = EditorPrefs.GetBool(KEY_EDITORPREF_IS_ENABLED, true);

				s_prefsLoaded = true;
			}
		}

		private static void SaveAllPreferenceValues()
		{
			EditorPrefs.SetBool(KEY_EDITORPREF_IS_ENABLED, s_isEnabled);

			RaisePreferencesChanged();
		}

		public UserPreferences(string path, SettingsScope scope = SettingsScope.User)
			: base(path, scope)
		{
			LoadAllPreferenceValues();
		}

		public override void OnGUI(string searchContext)
		{
			s_isEnabled = EditorGUILayout.Toggle("Enabled", s_isEnabled);
			EditorGUILayout.EnumPopup()

			if (GUI.changed)
			{
				SaveAllPreferenceValues();
			}
		}

		// Register the SettingsProvider
		[SettingsProvider]
		public static SettingsProvider CreateAssetGraphUserPreference()
		{
			var provider = new UserPreferences("Preferences/Auto Dark Theme")
			{
				keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
			};

			// Automatically extract all keywords from the Styles.
			return provider;
		}
	}

}