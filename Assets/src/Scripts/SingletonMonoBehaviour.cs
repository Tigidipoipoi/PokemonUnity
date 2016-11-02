using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
    where T : SingletonMonoBehaviour<T>
{
    public static T Instance
    {
        get
        {
            if (instance == null)
                Instantiate();

            return instance;
        }
    }

    protected static T instance;

    public static bool IsInstantiated { get { return instance != null; } }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = (T)this;
        else
        {
            if (instance != (T)this)
            {
                Debug.LogWarningFormat("[SingletonMonobehaviour] Instance not null at initialization: there can be only one instance of {0} on the map -> Destroying", GetType().ToString());
                Destroy(gameObject);

                return;
            }
        }
    }

    public static void Instantiate()
    {
        if (instance == null)
        {
            System.Type type = typeof(T);
            var go = new GameObject(type.Name, type);
            instance = go.GetComponent<T>();
        }
    }

    public static void Free()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }
}
