using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

using System.Diagnostics;

namespace Hrtzz.BuildTools
{
    //  TODO : Write Functionality for other platforms.
    public static class HrtzzBuildPipeline
    {
        static string [] scenesToBuild = GetScenesToBuild ();

        public static string APP_NAME = PlayerSettings.productName;
        public static string TARGET_DIR = "Builds";
        public static string VERSION_SETTINGS_LOCATION = Application.dataPath + "/HrtzzUtilities/BuildTools/Editor/BuildVersionSettings.json";
        #region Android Build Settings.

        //TODO : Must be modified according to per project's need.
        static string KEYSTORE_NAME = "Keystore/chancemansion.keystore";
        static string KEYSTORE_PASSWORD = "mansion@chance#123";

        static string KEY_ALLIAS_NAME = "chancemansion";
        static string KEY_ALLIAS_PASSWORD = "mansion@chance#123";
        static string BUNDLE_IDENTIFIER_ANDROID = "com.yesgnome.chancemansion";
        #endregion

        #region XCode Build Settings. TODO : Must be modified according to per project need.
        static string sourcePath = "PostBuildFiles/XCodeFiles/ToBeCopied/";
        static string entitlementsFileName = "";
        static string DEVELOPER_TEAM_ID = "67QV3GK84G";
        static string PROVISIONING_PROFILE_ID = "87bf0096-eca8-422a-9574-5ee8e53aa5fe";
        static string BUNDLE_IDENTIFIER_IOS = "com.yesgnome.chancemansion";
        #endregion

        [MenuItem ("BuildTools/Build APK", priority = 0)]
        private static void BuildApk ()
        {
            string buildPath = InitialiseAndroidBuildSettings ();
            if (!string.IsNullOrWhiteSpace (buildPath))
            {
                StartBuild (scenesToBuild, buildPath, BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
            }
        }

        [MenuItem ("BuildTools/Build & Run APK", priority = 0)]
        private static void BuildAndRunApk ()
        {
            string buildPath = InitialiseAndroidBuildSettings ();
            if (!string.IsNullOrWhiteSpace (buildPath))
            {
                StartBuild (scenesToBuild, buildPath, BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.AutoRunPlayer);
            }
        }
        //  Creates APK Build.
        private static string InitialiseAndroidBuildSettings ()
        {
            if (!File.Exists (VERSION_SETTINGS_LOCATION))
            {
                UnityEngine.Debug.Log ("Version settings does not exists");
                return null;
            }

            BuildVersionSettings settings = JsonUtility.FromJson<BuildVersionSettings> (File.ReadAllText (VERSION_SETTINGS_LOCATION));
            string buildPath = TARGET_DIR + "/APKs/" + settings.buildPhase.ToString () + "/" + APP_NAME + "_" + settings.Version + "[" + DateTime.Now.ToString ("yyyy'-'MM'-'dd']''['hh'-'mmtt") + "]" + ".apk";

            if (settings.buildType == BuildType.Release.ToString ())
            {
                //PlayerSettings.Android.keystoreName = KEYSTORE_NAME;
                PlayerSettings.Android.keystorePass = KEYSTORE_PASSWORD;
                PlayerSettings.Android.keyaliasName = KEY_ALLIAS_NAME;
                PlayerSettings.Android.keyaliasPass = KEY_ALLIAS_PASSWORD;
            }

            PlayerSettings.applicationIdentifier = BUNDLE_IDENTIFIER_ANDROID;
            PlayerSettings.Android.bundleVersionCode = settings.bundleVersion;
            PlayerSettings.bundleVersion = settings.v1 + "." + settings.v2 + "." + settings.v3;
            return buildPath;
        }


        [MenuItem ("BuildTools/Build XCODE", priority = 0)]
        private static void ExecuteIOSBuild ()
        {
            BuildVersionSettings settings = JsonUtility.FromJson<BuildVersionSettings> (File.ReadAllText (VERSION_SETTINGS_LOCATION));
            string dir = TARGET_DIR + "/XCODE/" + APP_NAME + "[" + DateTime.Now.ToString ("yyyy'-'MM'-'dd']'hh'-'mmtt");
            string modifiedDir = dir.Replace (" ", "_");

            //PlayerSettings.iOS.appleDeveloperTeamID = DEVELOPER_TEAM_ID;
            //PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            //PlayerSettings.iOS.iOSManualProvisioningProfileID = PROVISIONING_PROFILE_ID;

            PlayerSettings.applicationIdentifier = BUNDLE_IDENTIFIER_IOS;
            PlayerSettings.iOS.buildNumber = settings.bundleVersion.ToString ();

            PlayerSettings.bundleVersion = settings.v1 + "." + settings.v2 + "." + settings.v3;

            StartBuild (scenesToBuild, modifiedDir, BuildTargetGroup.iOS, BuildTarget.iOS, BuildOptions.None);
        }

        //  Returns all the enabled scenes in the build settings,
        private static string [] GetScenesToBuild ()
        {
            return EditorBuildSettings.scenes.Where (s => s.enabled).Select (s => s.path).ToArray ();
        }

        private static void StartBuild (string [] _scenes, string _targetDir, BuildTargetGroup _buildTargetGroup, BuildTarget _buildTarget, BuildOptions _buildOptions)
        {
            //  Switch the project to the targeted platform.
            EditorUserBuildSettings.SwitchActiveBuildTarget (_buildTargetGroup, _buildTarget);

            //  Initialize the build Options.
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = _scenes,
                locationPathName = _targetDir,
                targetGroup = _buildTargetGroup,
                target = _buildTarget,
                options = _buildOptions
            };
            //  Start the Build.
            BuildPipeline.BuildPlayer (buildPlayerOptions);
        }

        //  Testing Function.
        [MenuItem ("Build Tools/CheckCommand")]
        private static void CheckCommand ()
        {
            WriteBuildSettingToJson (UpdateBuildVersion ());
            //CopyAllContentFromSource(sourcePath, "TestFolder");
        }

        //  Copies all the content from the source path to destination path.
        public static void CopyAllContentFromSource (string _destPath)
        {
            //  Creates destination path if source path does not exists.
            if (!Directory.Exists (_destPath))
            {
                Directory.CreateDirectory (_destPath);
            }

            if (Directory.Exists (sourcePath))
            {
                DirectoryInfo dir = new DirectoryInfo (sourcePath);

                FileInfo [] files = dir.GetFiles ();                  //  Gets all the files in the source directory.
                DirectoryInfo [] subDirs = dir.GetDirectories ();     //  Gets all the sub directories from the source directory.

                //  Copy all the files.
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine (_destPath, file.Name);
                    file.CopyTo (tempPath, true);
                }

                //  Copy all the sub-directories.
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string tempPath = Path.Combine (_destPath, subDir.Name);
                    FileUtil.CopyFileOrDirectory (subDir.FullName, tempPath);
                }
                HrtzzFileBrowser.Open (_destPath);
            }

        }

#if UNITY_IOS
        //  Installs the required Cocoa Pods from the pods file..
        static void InstallCocoaPods (string _buildPath)
        {
            Process installPods = new Process ();
            installPods.StartInfo.UseShellExecute = false;
            installPods.StartInfo.RedirectStandardInput = true;
            installPods.StartInfo.RedirectStandardOutput = true;

            installPods.StartInfo.FileName = "open";
            installPods.StartInfo.Arguments = "InstallPods.command";
            installPods.StartInfo.WorkingDirectory = _buildPath;
            installPods.StartInfo.CreateNoWindow = true;
            installPods.Start ();
        }

        static void SetupXCodeProject (string _buildPath)
        {
            string projectPath = PBXProject.GetPBXProjectPath (_buildPath);

            PBXProject project = new PBXProject ();
            project.ReadFromString (File.ReadAllText (projectPath));

            string targetName = PBXProject.GetUnityTargetName ();
            string targetGuid = project.TargetGuidByName (targetName);

            // Move the entitlement file to from root directory to target directory.
            string entitleFileSource = _buildPath + "/" + entitlementsFileName;
            string entitleFileDestination = _buildPath + "/" + targetName + "/" + entitlementsFileName;
            FileUtil.MoveFileOrDirectory (entitleFileSource, entitleFileDestination);
            project.AddFile (targetName + "/" + entitlementsFileName, entitlementsFileName);
            project.AddBuildProperty (targetGuid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/NativeFramework");

            ProjectCapabilityManager capabilityManager = new ProjectCapabilityManager (projectPath, entitlementsFileName, targetName);
            capabilityManager.AddBackgroundModes (BackgroundModesOptions.RemoteNotifications);
            capabilityManager.WriteToFile ();

            project.AddBuildProperty (targetGuid, "CODE_SIGN_ENTITLEMENTS", targetName + "/" + entitlementsFileName);
            project.AddCapability (targetGuid, PBXCapabilityType.GameCenter);
            project.AddCapability (targetGuid, PBXCapabilityType.InAppPurchase);
            File.WriteAllText (projectPath, project.WriteToString ());
        }
#endif

        public static void WriteBuildSettingToJson (BuildVersionSettings settings)
        {
            string settingJson = JsonUtility.ToJson (settings, true);
            if (!File.Exists (VERSION_SETTINGS_LOCATION))
            {
                File.CreateText (VERSION_SETTINGS_LOCATION);
            }

            File.WriteAllText (VERSION_SETTINGS_LOCATION, settingJson);
            UnityEngine.Debug.Log (settings.Version);
            AssetDatabase.Refresh ();
        }

        //  Updates the version of Build
        public static BuildVersionSettings UpdateBuildVersion ()
        {
            BuildVersionSettings versionSetting = JsonUtility.FromJson<BuildVersionSettings> (File.ReadAllText (VERSION_SETTINGS_LOCATION));

            if ((versionSetting.v3 + 1) >= 10)
            {
                versionSetting.v3 = 0;
                if ((versionSetting.v2 + 1) >= 10)
                {
                    versionSetting.v2 = 0;
                    versionSetting.v1++;
                    return versionSetting;
                }
                else
                {
                    versionSetting.v2++;
                    return versionSetting;
                }
            }
            versionSetting.v3++;
            return versionSetting;
        }
    }


    public enum BuildPhase { Internal, Alpha, Beta, Final }
    public enum BuildType { Development, Release }

    [Serializable]
    public class BuildVersionSettings
    {
        public string buildPhase = BuildPhase.Alpha.ToString ();
        public string buildType = BuildType.Development.ToString ();
        public int bundleVersion = 1;
        public int v1 = 1, v2 = 0, v3 = 0;
        public string Version
        {
            get
            {
                return "v" + v1.ToString () + "." + v2.ToString () + "." + v3.ToString () + "(" + bundleVersion + ")" + "[" + buildPhase + "]" + "[" + buildType + "]";
            }
        }
    }
}
