using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using AutoDarkTheme.Windows;

namespace AutoDarkTheme
{
	[InitializeOnLoad]
	public class AutoDarkTheme
	{
#if UNITY_EDITOR_WIN
		private static RegistryMonitor registryMonitor;
#endif

		static AutoDarkTheme()
		{
#if UNITY_EDITOR_WIN
			// Windows: Watch system theme changes in registry
			registryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
			registryMonitor.RegChanged += new EventHandler(OnRegChanged);
			registryMonitor.Start();
#endif

			// Set correct theme on start
			SetEditorThemeFromSystemTheme();
		}

		~AutoDarkTheme() {
#if UNITY_EDITOR_WIN
			registryMonitor.Stop();
#endif
		}

		private static void Update()
		{
			SetEditorThemeFromSystemTheme();
			// Remove from update loop, this should only be called once after the system theme changes
			EditorApplication.update -= Update;
		}

		private static void OnRegChanged(object sender, EventArgs e)
		{
			// Add Update() method to update loop, calling SwitchSkinAndRepaintAllViews() alone doesn't work
			EditorApplication.update += Update;
		}

		private static void SetEditorThemeFromSystemTheme()
        {
#if UNITY_EDITOR_WIN
			var appsUseLightTheme = registryMonitor.ReadDword("AppsUseLightTheme");

			if (appsUseLightTheme == 1 && EditorGUIUtility.isProSkin)
			{
				Debug.Log("Editor: Switching to light theme....");

				EditorPrefs.SetInt("UserSkin", 0);
				InternalEditorUtility.SwitchSkinAndRepaintAllViews();
			}
			else if (appsUseLightTheme == 0 && !EditorGUIUtility.isProSkin)
			{
				Debug.Log("Editor: Switching to dark theme...");
				EditorPrefs.SetInt("UserSkin", 1);
				InternalEditorUtility.SwitchSkinAndRepaintAllViews();

			}
#endif
		}
	}
}

