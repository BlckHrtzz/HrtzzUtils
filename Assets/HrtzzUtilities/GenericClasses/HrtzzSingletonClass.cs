using UnityEngine;

public class HrtzzSingletonClass<T> : MonoBehaviour where T : Component
{
    static bool IsQuiting = false;
    private static T instance;
    public static T Instance
    {
        get
        {
            if (IsQuiting)
            {
                return null;
            }

            if (!instance)
            {
                instance = FindObjectOfType<T>();
                if (!instance)
                {
                    GameObject obj = new GameObject
                    {
                        name = typeof(T).Name
                    };
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public bool isPersistent = false;

    string instanceName = typeof(T).Name;

    protected virtual void Awake()
    {
        //  Toggles the Debug State
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        if (instance == null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log(instanceName + " Instance Missing. Creating new Instance of " + instanceName + ".");
            }
            instance = this as T;
            if (Debug.isDebugBuild)
            {
                Debug.Log(instanceName + " Instance created successfully!:).");
            }
        }
        else if (instance != this)
        {
            Debug.Log(instanceName + " already exists!...");
            Destroy(gameObject);
            return;
        }

        if (isPersistent)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        IsQuiting = true;
    }
}
