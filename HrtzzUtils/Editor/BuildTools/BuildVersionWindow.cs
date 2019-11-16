using UnityEngine;
using UnityEditor;
namespace Hrtzz.BuildTools
{
    public class HrtzzBuildVersionWindow : EditorWindow
    {
        public static BuildType currentBuildType = BuildType.Development;
        public static BuildPhase currentBuildPhase = BuildPhase.Alpha;
        public static int bundleVersion = 1;

        static BuildVersionSettings settings;

        [MenuItem ("Build Tools/Version Settings")]
        public static void ShowVersionSettings ()
        {
            settings = JsonUtility.FromJson<BuildVersionSettings> (System.IO.File.ReadAllText (BuildPipeline.VERSION_SETTINGS_LOCATION));
            currentBuildType = (BuildType) System.Enum.Parse (typeof (BuildType), settings.buildType);
            currentBuildPhase = (BuildPhase) System.Enum.Parse (typeof (BuildPhase), settings.buildPhase);
            bundleVersion = settings.bundleVersion;

            HrtzzBuildVersionWindow window = GetWindow<HrtzzBuildVersionWindow> ("Version Settings");
            window.minSize = window.maxSize = new Vector2 (300, 140);
        }

        private void OnGUI ()
        {
            GUILayout.Space (5);
            currentBuildType = (BuildType) EditorGUILayout.EnumPopup ("Build Type", currentBuildType);
            currentBuildPhase = (BuildPhase) EditorGUILayout.EnumPopup ("Build Phase", currentBuildPhase);
            bundleVersion = EditorGUILayout.IntField ("Bundle Version", bundleVersion);

            //EditorGUILayout.BeginVertical();
            if (GUILayout.Button ("Reset Build Settings", GUILayout.ExpandHeight (true)))
            {
                settings.buildPhase = BuildPhase.Alpha.ToString ();
                settings.buildType = BuildType.Development.ToString ();
                settings.bundleVersion = 1;
                settings.v1 = 1;
                settings.v2 = 0;
                settings.v3 = 0;
                BuildPipeline.WriteBuildSettingToJson (settings);
                ShowVersionSettings ();
            }

            if (GUILayout.Button ("Update Build Settings", GUILayout.ExpandHeight (true)))
            {
                settings.buildType = currentBuildType.ToString ();
                settings.buildPhase = currentBuildPhase.ToString ();
                settings.bundleVersion = bundleVersion;
                BuildPipeline.WriteBuildSettingToJson (settings);
            }
            //EditorGUILayout.BeginVertical();

        }
    }
}
