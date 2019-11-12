using UnityEngine;

public class HrtzzAutoSingleton<T> : MonoBehaviour where T : Component
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<T> ();
                if (!m_Instance)
                {
                    GameObject obj = new GameObject
                    {
                        name = typeof (T).Name
                    };
                    m_Instance = obj.AddComponent<T> ();
                }
            }
            return m_Instance;
        }
    }

    public bool isPersistent = false;
    //private string m_InstanceName = typeof (T).Name;

    protected virtual void Awake ()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
        else if (m_Instance != this)
        {
            Destroy (gameObject);
            return;
        }

        if (isPersistent)
        {
            DontDestroyOnLoad (gameObject);
        }
    }
}
