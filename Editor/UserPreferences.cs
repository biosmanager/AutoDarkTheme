using System;
using UnityEngine;
using UnityEditor;

namespace AutoDarkTheme
{
	class UserPreferences : SettingsProvider
	{
		static readonly string KEY_EDITORPREF_IS_ENABLED = "AutoDarkTheme.IsEnabled";
		static readonly string KEY_EDITORPREF_MODE = "AutoDarkTheme.Mode";

		public enum AutoThemeMode
        {
			System,
			Time
        }

		private static bool s_prefsLoaded = false;

		private static bool s_isEnabled;
		private static AutoThemeMode s_mode;

		internal class Styles
		{
			public static readonly GUIContent enabled = EditorGUIUtility.TrTextContent("Enabled");
			public static readonly GUIContent mode = EditorGUIUtility.TrTextContent("Change theme based on");
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

		public static AutoThemeMode Mode 
		{
			get 
			{
				LoadAllPreferenceValues();
				return s_mode;
			}
			set 
			{
				s_mode = value;
				SaveAllPreferenceValues();
			}
		} 

		private static void LoadAllPreferenceValues()
		{
			if (!s_prefsLoaded)
			{
				s_isEnabled = EditorPrefs.GetBool(KEY_EDITORPREF_IS_ENABLED, true);
				s_mode = (AutoThemeMode)EditorPrefs.GetInt(KEY_EDITORPREF_MODE);

				s_prefsLoaded = true;
			}
		}

		private static void SaveAllPreferenceValues()
		{
			EditorPrefs.SetBool(KEY_EDITORPREF_IS_ENABLED, s_isEnabled);
			EditorPrefs.SetInt(KEY_EDITORPREF_MODE, (int)s_mode);

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
			s_mode = (AutoThemeMode)EditorGUILayout.EnumPopup("Change theme based on", s_mode);

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