using UnityEngine;

public abstract class MonoSingletion<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                {
                    Debug.LogError("找不到" + typeof(T).ToString());
                }
            }

            return instance;
        }
    }
}