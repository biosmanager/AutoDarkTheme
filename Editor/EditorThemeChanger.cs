using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace AutoDarkTheme
{
    public static class EditorThemeChanger
    {
        private enum Theme
        {
            Light,
            Dark
        }


        private static Theme themeToSet;


        public static void SetLightTheme()
        {
            themeToSet = Theme.Light;
            EditorApplication.update += EditorThemeUpdate;
        }

        public static void SetDarkTheme()
        {
            themeToSet = Theme.Dark;
            EditorApplication.update += EditorThemeUpdate;
        }

        private static void EditorThemeUpdate()
        {
            if (themeToSet == Theme.Light && EditorGUIUtility.isProSkin)
            {
                Debug.Log("Auto Dark Theme: Switching to light theme.");
                EditorPrefs.SetInt("UserSkin", 0);
                InternalEditorUtility.SwitchSkinAndRepaintAllViews();
            }
            else if (themeToSet == Theme.Dark && !EditorGUIUtility.isProSkin)
            {
                Debug.Log("Auto Dark Theme: Switching to dark theme.");
                EditorPrefs.SetInt("UserSkin", 1);
                InternalEditorUtility.SwitchSkinAndRepaintAllViews();
            }

            EditorApplication.update -= EditorThemeUpdate;
        }

    }
}
