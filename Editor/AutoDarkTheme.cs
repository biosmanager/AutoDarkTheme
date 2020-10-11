using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using AutoDarkTheme.Schedule;
using AutoDarkTheme.Schedule.ScheduledItems;

#if UNITY_EDITOR_WIN
using AutoDarkTheme.Windows;
#endif

namespace AutoDarkTheme
{
    [InitializeOnLoad]
    public class AutoDarkTheme
    {
#if UNITY_EDITOR_WIN
        private static RegistryMonitor registryMonitor;
#endif

        private static ScheduleTimer timer;

        static AutoDarkTheme()
        {
            UserPreferences.PreferencesChanged += new EventHandler(OnSettingsChanged);

            MonitorThemeChanges();
        }

        ~AutoDarkTheme()
        {
#if UNITY_EDITOR_WIN
            registryMonitor?.Stop();
#endif
            timer?.Stop();
        }


        private static void Update()
        {
            SetEditorThemeFromSystemTheme();
            // Remove from update loop, this should only be called once after the system theme changes
            EditorApplication.update -= Update;
        }

        private static void OnSettingsChanged(object sender, EventArgs e)
        {
            Debug.Log(UserPreferences.IsEnabled);
            MonitorThemeChanges();
        }

        private static void OnRegChanged(object sender, EventArgs e)
        {
            // Add Update() method to update loop, calling SwitchSkinAndRepaintAllViews() alone doesn't work
            EditorApplication.update += Update;
        }


        private static void MonitorThemeChanges()
        {
#if UNITY_EDITOR_WIN
            registryMonitor?.Stop();
#endif

            if (UserPreferences.IsEnabled)
            {
                if (UserPreferences.Mode == UserPreferences.AutoThemeMode.System)
                {
#if UNITY_EDITOR_WIN
                    // Windows: Watch system theme changes in registry
                    registryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
                    registryMonitor.RegChanged += new EventHandler(OnRegChanged);
                    registryMonitor.Start();

                    // Set current system theme on start/when enabled
                    SetEditorThemeFromSystemTheme();
#endif
                }
                else if (UserPreferences.Mode == UserPreferences.AutoThemeMode.Time)
                {
                    var lightThemeSchedule = new ScheduledTime("Daily", "4:17 PM");
                    var darkThemeSchedule = new ScheduledTime("Daily", "6:00 PM");

                    timer = new ScheduleTimer();
                    timer.AddJob(lightThemeSchedule, new Action(SetLightTheme));
                    timer.AddJob(darkThemeSchedule, new Action(SetDarkTheme));
                    timer.Start();
                }
            }
        }

        private static void SetEditorThemeFromSystemTheme()
        {
#if UNITY_EDITOR_WIN
            var appsUseLightTheme = registryMonitor.ReadDword("AppsUseLightTheme");

            if (appsUseLightTheme == 1 && EditorGUIUtility.isProSkin)
            {
                SetLightTheme();
            }
            else if (appsUseLightTheme == 0 && !EditorGUIUtility.isProSkin)
            {
                SetDarkTheme();
            }
#endif
        }

        private static void SetLightTheme()
        {
            Debug.Log("Editor: Switching to light theme....");

            EditorPrefs.SetInt("UserSkin", 0);
            InternalEditorUtility.SwitchSkinAndRepaintAllViews();
        }

        private static void SetDarkTheme()
        {
            Debug.Log("Editor: Switching to dark theme...");
            EditorPrefs.SetInt("UserSkin", 1);
            InternalEditorUtility.SwitchSkinAndRepaintAllViews();
        }
    }
}
