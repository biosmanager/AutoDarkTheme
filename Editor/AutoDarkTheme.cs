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
            UserPreferences.PreferencesChanged += (sender, args) => MonitorThemeChanges();

            MonitorThemeChanges();
        }

        ~AutoDarkTheme()
        {
#if UNITY_EDITOR_WIN
            registryMonitor?.Stop();
#endif
            timer?.Stop();
        }

        private static void MonitorThemeChanges()
        {
#if UNITY_EDITOR_WIN
            registryMonitor?.Stop();
#endif

            timer?.Stop();

            if (UserPreferences.IsEnabled)
            {
                if (UserPreferences.Mode == UserPreferences.AutoThemeMode.System)
                {
#if UNITY_EDITOR_WIN
                    // Windows: Watch system theme changes in registry
                    registryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
                    registryMonitor.RegChanged += (sender, args) => SetEditorThemeFromSystemTheme();
                    registryMonitor.Start();

                    // Set current system theme on start/when enabled
                    SetEditorThemeFromSystemTheme();
#endif
                }
                else if (UserPreferences.Mode == UserPreferences.AutoThemeMode.Time)
                {
                    // Default light theme time
                    var lightThemeTime = UserPreferences.DEFAULT_LIGHT_THEME_TIME;
                    // Default dark theme time
                    var darkThemeTime = UserPreferences.DEFAULT_DARK_THEME_TIME;

                    // Parse light theme time
                    try
                    {
                        lightThemeTime = TimeSpan.Parse(UserPreferences.LightThemeTime);
                    }
                    catch (Exception e) when (e is FormatException || e is OverflowException)
                    {
                        Debug.LogError($"Auto Dark Theme: Invalid light theme time: {UserPreferences.LightThemeTime}, reverting to {UserPreferences.DEFAULT_LIGHT_THEME_TIME}.");
                    }

                    // Parse dark theme time
                    try
                    {
                        darkThemeTime = TimeSpan.Parse(UserPreferences.DarkThemeTime);
                    }
                    catch (Exception e) when (e is FormatException || e is OverflowException)
                    {
                        Debug.LogError($"Auto Dark Theme: Invalid dark theme time: {UserPreferences.LightThemeTime}, reverting to {UserPreferences.DEFAULT_DARK_THEME_TIME}.");
                    }

                    // Check current time and set theme
                    // UNDONE: When light theme time is later than dark theme time, this doesn't work.
                    if (DateTime.Now.TimeOfDay >= lightThemeTime && DateTime.Now.TimeOfDay < darkThemeTime)
                    {
                        EditorThemeChanger.SetLightTheme();
                    }
                    else
                    {
                        EditorThemeChanger.SetDarkTheme();
                    }

                    // Schedule theme changes
                    var lightThemeSchedule = new ScheduledTime(EventTimeBase.Daily, lightThemeTime);
                    var darkThemeSchedule = new ScheduledTime(EventTimeBase.Daily, darkThemeTime);
                    timer = new ScheduleTimer();
                    timer.AddJob(lightThemeSchedule, new Action(EditorThemeChanger.SetLightTheme));
                    timer.AddJob(darkThemeSchedule, new Action(EditorThemeChanger.SetDarkTheme));
                    timer.Start();
                }
            }
        }

        private static void SetEditorThemeFromSystemTheme()
        {
#if UNITY_EDITOR_WIN
            var appsUseLightTheme = registryMonitor.ReadDword("AppsUseLightTheme");

            if (appsUseLightTheme == 1)
            {
                EditorThemeChanger.SetLightTheme();
            }
            else if (appsUseLightTheme == 0)
            {
                EditorThemeChanger.SetDarkTheme();
            }
#endif
        }
    }
}
