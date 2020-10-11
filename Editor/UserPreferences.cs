using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AutoDarkTheme
{
	class UserPreferences : SettingsProvider
	{
		public enum AutoThemeMode
		{
			System,
			Time
		}

		internal class Styles
		{
			public static readonly GUIContent enabled = EditorGUIUtility.TrTextContent("Enabled");
			public static readonly GUIContent mode = EditorGUIUtility.TrTextContent("Change theme based on");
		}


		static readonly string KEY_EDITORPREF_IS_ENABLED = "AutoDarkTheme.IsEnabled";
		static readonly string KEY_EDITORPREF_MODE = "AutoDarkTheme.Mode";
		static readonly string KEY_EDITORPREF_LIGHT_THEME_TIME = "AutoDarkTheme.LightThemeTime";
		static readonly string KEY_EDITORPREF_DARK_THEME_TIME = "AutoDarkTheme.DarkThemeTime";


		private static bool s_prefsLoaded = false;
		private static bool s_isEnabled;
		private static AutoThemeMode s_mode;
		private static string s_lightThemeTime;
		private static string s_darkThemeTime;


		public static readonly bool DEFAULT_IS_ENABLED = true;
		public static readonly AutoThemeMode DEFAULT_MODE = AutoThemeMode.System;
		public static readonly TimeSpan DEFAULT_LIGHT_THEME_TIME = new TimeSpan(07, 00, 00);
		public static readonly TimeSpan DEFAULT_DARK_THEME_TIME = new TimeSpan(18, 00, 00);


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

		public static string LightThemeTime
        {
			get
            {
				LoadAllPreferenceValues();
				return s_lightThemeTime;
            }
			set
            {
				s_lightThemeTime = value;
				SaveAllPreferenceValues();
            }
        }

		public static string DarkThemeTime
		{
			get
			{
				LoadAllPreferenceValues();
				return s_darkThemeTime;
			}
			set
			{
				s_darkThemeTime = value;
				SaveAllPreferenceValues();
			}
		}


		public static event EventHandler PreferencesChanged;
		private static void RaisePreferencesChanged()
        {
			var handler = PreferencesChanged;
			handler?.Invoke(typeof(UserPreferences), EventArgs.Empty);
        }


		private static void LoadAllPreferenceValues()
		{
			if (!s_prefsLoaded)
			{
				s_isEnabled = EditorPrefs.GetBool(KEY_EDITORPREF_IS_ENABLED, DEFAULT_IS_ENABLED);
				s_mode = (AutoThemeMode)EditorPrefs.GetInt(KEY_EDITORPREF_MODE, (int)DEFAULT_MODE);
				s_lightThemeTime = EditorPrefs.GetString(KEY_EDITORPREF_LIGHT_THEME_TIME, DEFAULT_LIGHT_THEME_TIME.ToString());
				s_darkThemeTime = EditorPrefs.GetString(KEY_EDITORPREF_DARK_THEME_TIME, DEFAULT_DARK_THEME_TIME.ToString());

				s_prefsLoaded = true;
			}
		}

		private static void SaveAllPreferenceValues()
		{
			EditorPrefs.SetBool(KEY_EDITORPREF_IS_ENABLED, s_isEnabled);
			EditorPrefs.SetInt(KEY_EDITORPREF_MODE, (int)s_mode);
			EditorPrefs.SetString(KEY_EDITORPREF_LIGHT_THEME_TIME, s_lightThemeTime);
			EditorPrefs.SetString(KEY_EDITORPREF_DARK_THEME_TIME, s_darkThemeTime);

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

			if (s_isEnabled)
            {
				s_mode = (AutoThemeMode)EditorGUILayout.EnumPopup("Change theme based on", s_mode);

				if (s_mode == AutoThemeMode.Time)
				{
					s_lightThemeTime = EditorGUILayout.TextField("Enable light theme at", s_lightThemeTime);
					s_darkThemeTime = EditorGUILayout.TextField("Enable dark theme at", s_darkThemeTime);
				}
            }

			EditorGUILayout.Space();

			if (GUILayout.Button("Apply"))
			{
				SaveAllPreferenceValues();
			}
		}

        public override void OnDeactivate()
		{
			SaveAllPreferenceValues();
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