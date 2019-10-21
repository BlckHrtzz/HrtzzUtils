using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class BuildPlayer
{
    static string adbLocation;
    static string bundleIdent = PlayerSettings.applicationIdentifier;

    [MenuItem("Build/Run Last APK")]
    public static void PushToAndroid()
    {
        string apkLocation = PlayerPrefs.GetString("APK location");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
            apkLocation = EditorUtility.OpenFilePanel("Find APK", Environment.CurrentDirectory, "apk");

        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
        {
            Debug.LogError("Cannot find .apk file.");
            return;
        }
        PlayerPrefs.SetString("APK location", apkLocation);

        adbLocation = PlayerPrefs.GetString("ADB");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(adbLocation))
        {
            adbLocation = EditorUtility.OpenFilePanel("Android Debug Bridge", Environment.CurrentDirectory, "exe");
        }
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(adbLocation))
        {
            Debug.LogError("Cannot find adb.exe.");
            return;
        }
        PlayerPrefs.SetString("ADB", adbLocation);

        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = adbLocation,
            Arguments = string.Format("install -r \"{0}\"", apkLocation),
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };
        Process adbPushProcess = Process.Start(info);
        if (adbPushProcess != null)
        {
            adbPushProcess.EnableRaisingEvents = true;
            adbPushProcess.Exited += RunApp;
        }
        else
        {
            Debug.LogError("Error starting adb");
        }
    }

    public static void RunApp(object o, EventArgs args)
    {
        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = adbLocation,
            Arguments = string.Format("shell am start -n " + bundleIdent + "/com.unity3d.player.UnityPlayerNativeActivity"),
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };

        Process.Start(info);
    }

    [MenuItem("Build/Update APK Path")]
    public static void ChangeAPK()
    {
        PlayerPrefs.SetString("APK location", EditorUtility.OpenFilePanel("Find APK", Environment.CurrentDirectory, "apk"));
    }
}