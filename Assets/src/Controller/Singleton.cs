using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Singleton<T> : IDisposable
    where T : class, IDisposable, new()
{
    protected static T instance;
    protected static bool isInitialized = false;

    public static bool IsInitialized
    {
        get
        {
            return isInitialized;
        }
        protected set
        {
            isInitialized = value;
        }
    }

    public static T Instance
    {
        get
        {
            if (instance == null)
                Instanciate();

            return instance;
        }
    }

    public static void Instanciate()
    {
        instance = new T();
    }

    public static void Free()
    {
        isInitialized = false;
        if (instance != null)
            instance.Dispose();

        instance = null;
    }

    public virtual void Dispose()
    {

    }
}
