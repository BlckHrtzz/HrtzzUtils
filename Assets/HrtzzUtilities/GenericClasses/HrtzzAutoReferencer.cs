using System.Linq;
using UnityEngine;

public class HrtzzAutoReferencer<T> : MonoBehaviour where T : HrtzzAutoReferencer<T>
{
#if UNITY_EDITOR
    protected virtual void Reset()
    {
        foreach (var field in typeof(T).GetFields().Where(field => field.GetValue(this) == null))
        {
            Transform obj;

            if (transform.name == field.Name)
            {
                obj = transform;
            }
            else
            {
                obj = FindReference(transform, field.Name);
            }

            if (obj != null)
            {
                field.SetValue(this, obj.GetComponent(field.FieldType));
                Debug.Log("<color=green> Reference Initialised Successfully </color>");
            }
            else
                Debug.LogWarning("Reference Not Found");
        }
    }

    Transform FindReference(Transform parent, string fieldName)
    {
        Transform objToReturn = null;
        int childCount = parent.childCount;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == fieldName)
            {
                objToReturn = child;
                return objToReturn;
            }
        }

        foreach (Transform child in parent)
        {
            objToReturn = FindReference(child, fieldName);
            if (objToReturn != null)
            {
                return objToReturn;
            }
        }
        return objToReturn;
    }
#endif
}
