using UnityEngine;
using System;
using System.IO;

public static class HrtzzJsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            Items = array
        };
        return JsonUtility.ToJson(wrapper);
    }

    // Deserializng with formatting.
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            Items = array
        };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    #region IO Helper Functions
    // Gets the file path depending on platform.
    private static string GetPath()
    {
        string path = "";
        #if UNITY_EDITOR
        path = Application.dataPath + "/Resources/JsonFiles/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        #endif

        #if UNITY_IOS && !UNITY_EDITOR
        path = Application.persistentDataPath;
        #endif

        #if UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath;
        #endif

        //Debug.Log("Path : " + path);
        return path + "/";
    }

    /// <summary>
    /// Saves a JSON string to Json File.
    /// </summary>
    /// <param name="fileName"> Name of File(without .json extension).</param>
    /// <param name="data"> string that needs to be saved</param>
    public static void SaveToJson(string fileName, string data)
    {
        File.WriteAllText(GetPath() + fileName + ".json", data);
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    public static string ReadFromJson(string fileName)
    {
        return File.ReadAllText(GetPath() + fileName + ".json");
    }
    #endregion
}
