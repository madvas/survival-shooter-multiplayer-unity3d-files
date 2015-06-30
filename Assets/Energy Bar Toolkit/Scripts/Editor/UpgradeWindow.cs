/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using EnergyBarToolkit;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class UpgradeWindow : EditorWindow {
    private const string LastQueryTime = "LastQueryTime";
    private const string DoNotShow = "DoNotShow";
    private const string QueryVersion = "QueryVersion";

    public static string queryVersion {
        get { return EditorPrefs.GetString(QueryVersion, "0"); }
        set { EditorPrefs.SetString(QueryVersion, value); }
    }

    public static bool doNotShow {
        get { return EditorPrefs.GetBool(DoNotShow, false); }
        set { EditorPrefs.SetBool(DoNotShow, value); }
    }

    public static float lastQueryTime {
        get { return EditorPrefs.GetFloat(LastQueryTime, float.MaxValue); }
        set { EditorPrefs.SetFloat(LastQueryTime, value);}
    }

    static UpgradeWindow() {
        EditorApplication.delayCall += () => {
            if (CanShow()) {
                ShowWindow();
            }
        };
    }

    public static void ShowWindow() {
        var w = GetWindow<UpgradeWindow>(true, "EBT Upgrade", true);
        w.minSize = new Vector2(540, 380);
    }

    private static bool CanShow() {
        try {
            if (queryVersion != Version.current) {
                return true; // always show if version is different
            }

            if (doNotShow) {
                return false; // the same version, do not show
            }

        
            if (EditorApplication.timeSinceStartup > lastQueryTime) {
                // most probably tring to show more than once in current editor instance
                return false;
            }

            return true;
        } finally {
            lastQueryTime = (float) EditorApplication.timeSinceStartup;
            queryVersion = Version.current;
        }
    }

#if EBT_DEBUG

    [MenuItem("Tools/EBT Debug/Upgrade Window/Show")]
    public static void DebugShow() {
        ShowWindow();
    }

    [MenuItem("Tools/EBT Debug/Upgrade Window/Reset")]
    public static void DebugReset() {
        doNotShow = false;
        lastQueryTime = float.MaxValue;
    }

    [MenuItem("Tools/EBT Debug/Upgrade Window/Can show?")]
    public static void DebugCanShow() {
        EditorUtility.DisplayDialog("Can Show?", CanShow() ? "Yes!" : "No!", "OK");
    }

#endif

    void OnGUI() {
        var style = new GUIStyle(GUI.skin.label);
        style.fontSize = 25;
        style.normal.textColor = Color.white;
        //style.fontStyle = FontStyle.Bold;
        GUI.color = new Color(1, 61 / 255f, 61 / 255f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, 50), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(10);
        GUILayout.Label("Energy Bar Toolkit Upgrade Info", style);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        style = new GUIStyle(GUI.skin.label);

        GUILayout.Label("Thank you for upgrading Energy Bar Toolkit to " + Version.current + "!");

        EditorGUILayout.Space();

        style.fontSize = 15;
        GUILayout.Label("Important!", style);

        MadGUI.Error("Remember to backup your project before upgrading Energy Bar Toolkit to newer version!");

        GUILayout.Label("Remember that...", style);

        MadGUI.Warning("If you see errors after upgrading Energy Bar Toolkit, please remove Energy Bar Toolkit " +
                       "directory and import it again.");

        MadGUI.Warning("OnGUI bars will not work with Unity 5 and those are no longer maintained.");

        GUILayout.Label("Tips", style);

        MadGUI.Info("Mesh bars are now deprecated. We strongly recommend to use new uGUI bars!");

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        doNotShow = EditorGUILayout.ToggleLeft("Do not show again until next upgrade", doNotShow);
        if (GUILayout.Button("Changelog")) {
            Application.OpenURL(string.Format("http://energybartoolkit.madpixelmachine.com/doc/{0}/changelog.html", Version.current));
        }
        EditorGUILayout.EndHorizontal();
    }
}